// Moirai Framework 一键安装脚本
// 使用方法：将本文件放入 Unity 工程的 Assets/Editor/ 目录，
// 脚本会自动检测并添加 OpenUPM Scoped Registry，然后导入框架。
// 安装完成后可删除本文件。
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Moirai.Installer
{
    public static class MoiraiFrameworkInstaller
    {
        private const string RegistryName = "Open UPM";
        private const string RegistryUrl = "https://package.openupm.com";
        private static readonly string[] RequiredScopes = { "com.cysharp", "com.tuyoogame" };

        private const string PackageName = "com.moirai.framework";
        private const string PackageGitUrl = "https://github.com/Lx34r/com.moirai.framework.git";

        [InitializeOnLoadMethod]
        private static void AutoInstall()
        {
            EditorApplication.delayCall += () => Install(false);
        }

        [MenuItem("Moirai/Install Framework")]
        private static void InstallFromMenu()
        {
            Install(true);
        }

        private static void Install(bool verbose)
        {
            try
            {
                string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
                if (!File.Exists(manifestPath))
                {
                    Debug.LogError("[MoiraiInstaller] 未找到 Packages/manifest.json");
                    return;
                }

                var manifest = Json.Deserialize(File.ReadAllText(manifestPath)) as Dictionary<string, object>;
                if (manifest == null)
                {
                    Debug.LogError("[MoiraiInstaller] 解析 manifest.json 失败");
                    return;
                }

                bool changed = false;
                changed |= EnsureScopedRegistry(manifest);
                changed |= EnsureDependency(manifest);

                if (changed)
                {
                    File.WriteAllText(manifestPath, Json.Serialize(manifest), new UTF8Encoding(false));
                    Debug.Log("[MoiraiInstaller] 已配置 Scoped Registry 并添加 Moirai Framework，正在解析包...");
                    UnityEditor.PackageManager.Client.Resolve();
                }
                else if (verbose)
                {
                    Debug.Log("[MoiraiInstaller] Scoped Registry 与 Moirai Framework 已配置，无需修改。");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[MoiraiInstaller] 安装失败: " + e);
            }
        }

        private static bool EnsureScopedRegistry(Dictionary<string, object> manifest)
        {
            bool changed = false;
            if (!(manifest.TryGetValue("scopedRegistries", out var regsObj) && regsObj is List<object> registries))
            {
                registries = new List<object>();
                manifest["scopedRegistries"] = registries;
            }
            else
            {
                registries = (List<object>)regsObj;
            }

            Dictionary<string, object> openUpm = null;
            foreach (var r in registries)
            {
                if (r is Dictionary<string, object> reg &&
                    reg.TryGetValue("url", out var url) &&
                    RegistryUrl.Equals(url as string, StringComparison.OrdinalIgnoreCase))
                {
                    openUpm = reg;
                    break;
                }
            }

            if (openUpm == null)
            {
                openUpm = new Dictionary<string, object>
                {
                    { "name", RegistryName },
                    { "url", RegistryUrl },
                    { "scopes", new List<object>() }
                };
                registries.Add(openUpm);
                changed = true;
            }

            if (!(openUpm.TryGetValue("scopes", out var scopesObj) && scopesObj is List<object> scopes))
            {
                scopes = new List<object>();
                openUpm["scopes"] = scopes;
                changed = true;
            }
            else
            {
                scopes = (List<object>)scopesObj;
            }

            foreach (var required in RequiredScopes)
            {
                if (!scopes.Contains(required))
                {
                    scopes.Add(required);
                    changed = true;
                }
            }

            return changed;
        }

        private static bool EnsureDependency(Dictionary<string, object> manifest)
        {
            if (!(manifest.TryGetValue("dependencies", out var depsObj) && depsObj is Dictionary<string, object> deps))
            {
                deps = new Dictionary<string, object>();
                manifest["dependencies"] = deps;
            }
            else
            {
                deps = (Dictionary<string, object>)depsObj;
            }

            if (deps.ContainsKey(PackageName))
            {
                return false;
            }

            deps[PackageName] = PackageGitUrl;
            return true;
        }

        #region MiniJSON
        // MiniJSON - public domain JSON parser/serializer (https://gist.github.com/darktable/1411710)
        private static class Json
        {
            public static object Deserialize(string json)
            {
                return json == null ? null : Parser.Parse(json);
            }

            public static string Serialize(object obj)
            {
                return Serializer.MakeSerialization(obj);
            }

            private sealed class Parser : IDisposable
            {
                private const string WordBreak = "{}[],:\"";
                private StringReader json;

                private Parser(string jsonString)
                {
                    json = new StringReader(jsonString);
                }

                public static object Parse(string jsonString)
                {
                    using (var instance = new Parser(jsonString))
                    {
                        return instance.ParseValue();
                    }
                }

                public void Dispose()
                {
                    json.Dispose();
                    json = null;
                }

                private Dictionary<string, object> ParseObject()
                {
                    var table = new Dictionary<string, object>();
                    json.Read(); // {
                    while (true)
                    {
                        switch (NextToken)
                        {
                            case Token.None:
                                return null;
                            case Token.Comma:
                                continue;
                            case Token.CurlyClose:
                                return table;
                            default:
                                string name = ParseString();
                                if (name == null) return null;
                                if (NextToken != Token.Colon) return null;
                                json.Read(); // :
                                table[name] = ParseValue();
                                break;
                        }
                    }
                }

                private List<object> ParseArray()
                {
                    var array = new List<object>();
                    json.Read(); // [
                    var parsing = true;
                    while (parsing)
                    {
                        Token nextToken = NextToken;
                        switch (nextToken)
                        {
                            case Token.None:
                                return null;
                            case Token.Comma:
                                continue;
                            case Token.SquaredClose:
                                parsing = false;
                                break;
                            default:
                                array.Add(ParseByToken(nextToken));
                                break;
                        }
                    }
                    return array;
                }

                private object ParseValue()
                {
                    return ParseByToken(NextToken);
                }

                private object ParseByToken(Token token)
                {
                    switch (token)
                    {
                        case Token.String:
                            return ParseString();
                        case Token.Number:
                            return ParseNumber();
                        case Token.CurlyOpen:
                            return ParseObject();
                        case Token.SquaredOpen:
                            return ParseArray();
                        case Token.True:
                            return true;
                        case Token.False:
                            return false;
                        case Token.Null:
                            return null;
                        default:
                            return null;
                    }
                }

                private string ParseString()
                {
                    var s = new StringBuilder();
                    json.Read(); // "
                    var parsing = true;
                    while (parsing)
                    {
                        if (json.Peek() == -1) break;
                        char c = NextChar;
                        switch (c)
                        {
                            case '"':
                                parsing = false;
                                break;
                            case '\\':
                                if (json.Peek() == -1)
                                {
                                    parsing = false;
                                    break;
                                }
                                c = NextChar;
                                switch (c)
                                {
                                    case '"':
                                    case '\\':
                                    case '/':
                                        s.Append(c);
                                        break;
                                    case 'b':
                                        s.Append('\b');
                                        break;
                                    case 'f':
                                        s.Append('\f');
                                        break;
                                    case 'n':
                                        s.Append('\n');
                                        break;
                                    case 'r':
                                        s.Append('\r');
                                        break;
                                    case 't':
                                        s.Append('\t');
                                        break;
                                    case 'u':
                                        var hex = new char[4];
                                        for (int i = 0; i < 4; i++) hex[i] = NextChar;
                                        s.Append((char)Convert.ToInt32(new string(hex), 16));
                                        break;
                                }
                                break;
                            default:
                                s.Append(c);
                                break;
                        }
                    }
                    return s.ToString();
                }

                private object ParseNumber()
                {
                    string number = NextWord;
                    if (number.IndexOf('.') == -1)
                    {
                        long.TryParse(number, out long parsedInt);
                        return parsedInt;
                    }
                    double.TryParse(number, out double parsedDouble);
                    return parsedDouble;
                }

                private void EatWhitespace()
                {
                    while (char.IsWhiteSpace(PeekChar))
                    {
                        json.Read();
                        if (json.Peek() == -1) break;
                    }
                }

                private char PeekChar => Convert.ToChar(json.Peek());

                private char NextChar => Convert.ToChar(json.Read());

                private string NextWord
                {
                    get
                    {
                        var word = new StringBuilder();
                        while (!IsWordBreak(PeekChar))
                        {
                            word.Append(NextChar);
                            if (json.Peek() == -1) break;
                        }
                        return word.ToString();
                    }
                }

                private Token NextToken
                {
                    get
                    {
                        EatWhitespace();
                        if (json.Peek() == -1) return Token.None;
                        switch (PeekChar)
                        {
                            case '{':
                                return Token.CurlyOpen;
                            case '}':
                                json.Read();
                                return Token.CurlyClose;
                            case '[':
                                return Token.SquaredOpen;
                            case ']':
                                json.Read();
                                return Token.SquaredClose;
                            case ',':
                                json.Read();
                                return Token.Comma;
                            case '"':
                                return Token.String;
                            case ':':
                                return Token.Colon;
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            case '-':
                                return Token.Number;
                        }
                        switch (NextWord)
                        {
                            case "false":
                                return Token.False;
                            case "true":
                                return Token.True;
                            case "null":
                                return Token.Null;
                        }
                        return Token.None;
                    }
                }

                private static bool IsWordBreak(char c)
                {
                    return char.IsWhiteSpace(c) || WordBreak.IndexOf(c) != -1;
                }

                private enum Token
                {
                    None,
                    CurlyOpen,
                    CurlyClose,
                    SquaredOpen,
                    SquaredClose,
                    Colon,
                    Comma,
                    String,
                    Number,
                    True,
                    False,
                    Null
                }
            }

            private sealed class Serializer
            {
                private readonly StringBuilder builder = new StringBuilder();
                private int indent;

                public static string MakeSerialization(object obj)
                {
                    var instance = new Serializer();
                    instance.SerializeValue(obj);
                    return instance.builder.ToString();
                }

                private void AppendIndent()
                {
                    builder.Append(' ', indent * 2);
                }

                private void SerializeValue(object value)
                {
                    if (value == null)
                    {
                        builder.Append("null");
                    }
                    else if (value is string str)
                    {
                        SerializeString(str);
                    }
                    else if (value is bool b)
                    {
                        builder.Append(b ? "true" : "false");
                    }
                    else if (value is IDictionary dict)
                    {
                        SerializeObject(dict);
                    }
                    else if (value is IList list)
                    {
                        SerializeArray(list);
                    }
                    else if (value is char c)
                    {
                        SerializeString(new string(c, 1));
                    }
                    else
                    {
                        SerializeOther(value);
                    }
                }

                private void SerializeObject(IDictionary obj)
                {
                    builder.Append("{\n");
                    indent++;
                    bool first = true;
                    foreach (object e in obj.Keys)
                    {
                        if (!first) builder.Append(",\n");
                        AppendIndent();
                        SerializeString(e.ToString());
                        builder.Append(": ");
                        SerializeValue(obj[e]);
                        first = false;
                    }
                    indent--;
                    builder.Append('\n');
                    AppendIndent();
                    builder.Append('}');
                }

                private void SerializeArray(IList anArray)
                {
                    builder.Append("[\n");
                    indent++;
                    bool first = true;
                    foreach (object obj in anArray)
                    {
                        if (!first) builder.Append(",\n");
                        AppendIndent();
                        SerializeValue(obj);
                        first = false;
                    }
                    indent--;
                    builder.Append('\n');
                    AppendIndent();
                    builder.Append(']');
                }

                private void SerializeString(string str)
                {
                    builder.Append('\"');
                    foreach (var c in str)
                    {
                        switch (c)
                        {
                            case '"':
                                builder.Append("\\\"");
                                break;
                            case '\\':
                                builder.Append("\\\\");
                                break;
                            case '\b':
                                builder.Append("\\b");
                                break;
                            case '\f':
                                builder.Append("\\f");
                                break;
                            case '\n':
                                builder.Append("\\n");
                                break;
                            case '\r':
                                builder.Append("\\r");
                                break;
                            case '\t':
                                builder.Append("\\t");
                                break;
                            default:
                                int codepoint = Convert.ToInt32(c);
                                if (codepoint >= 32 && codepoint <= 126)
                                {
                                    builder.Append(c);
                                }
                                else
                                {
                                    builder.Append("\\u");
                                    builder.Append(codepoint.ToString("x4"));
                                }
                                break;
                        }
                    }
                    builder.Append('\"');
                }

                private void SerializeOther(object value)
                {
                    if (value is float f)
                    {
                        builder.Append(f.ToString("R"));
                    }
                    else if (value is int || value is uint || value is long ||
                             value is sbyte || value is byte || value is short ||
                             value is ushort || value is ulong)
                    {
                        builder.Append(value);
                    }
                    else if (value is double || value is decimal)
                    {
                        builder.Append(Convert.ToDouble(value).ToString("R"));
                    }
                    else
                    {
                        SerializeString(value.ToString());
                    }
                }
            }
        }
        #endregion
    }
}
#endif
