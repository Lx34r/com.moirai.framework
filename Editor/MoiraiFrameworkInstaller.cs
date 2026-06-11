#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Moirai.Atropos.Installer.Editor
{
    public sealed class FrameworkInstallerWindow : EditorWindow
    {
        private const string MENU_PATH = "Tools/Settings/Install Framework";
        private const string TEMPLATES_PATH = "Packages/" + CORE_PACKAGE_NAME + "/Templates~";
        private const string NORMAL_TEMPLATE_NAME = "NormalTemplate";
        private const string HYBRID_TEMPLATE_NAME = "HybridTemplate";
        private const string NORMAL_TEMPLATE_PATH = TEMPLATES_PATH + "/" + NORMAL_TEMPLATE_NAME;
        private const string HYBRID_TEMPLATE_PATH = TEMPLATES_PATH + "/" + HYBRID_TEMPLATE_NAME;

        private const string CORE_PACKAGE_NAME = "com.moirai.framework";
        private const string URP_PACKAGE_NAME = "com.unity.render-pipelines.universal";
        private const string HYBRID_CLR_PACKAGE_NAME = "com.code-philosophy.hybridclr";
        private const string ENABLE_LOG_SYMBOL = "ENABLE_LOG";
        private const string ENABLE_HYBRID_CLR_SYMBOL = "ENABLE_HYBRIDCLR";

        private const string REQUIRED_REGISTRY_NAME = "Open UPM";
        private const string REQUIRED_REGISTRY_URL = "https://package.openupm.com";
        private const string REQUIRED_REGISTRY_SCOPE_CYSHARP = "com.cysharp";
        private const string REQUIRED_REGISTRY_SCOPE_TUYOO_GAME = "com.tuyoogame";
        private const string REQUIRED_REGISTRY_SCOPE_MOIRAI = "com.moirai";

        private const string MANIFEST_PATH = "Packages/manifest.json";
        private const string INSTALL_STATE_PATH = "ProjectSettings/MoiraiFrameworkInstaller.json";

        private static readonly string[] s_RequiredRegistryScopes =
        {
            REQUIRED_REGISTRY_SCOPE_CYSHARP,
            REQUIRED_REGISTRY_SCOPE_TUYOO_GAME,
            REQUIRED_REGISTRY_SCOPE_MOIRAI,
        };

        private static readonly string[] s_RuntimeAssetMarkers =
        {
            "Assets/Bundles",
            "Assets/YooAsset"
        };

        private static readonly string[] s_HybridAssetMarkers =
        {
            "Assets/Scripts/Hotfix",
            "Assets/HybridCLRGenerate",
            "Assets/Bundles/DLL"
        };

        private InstallCheckResult _checkResult;
        private TemplateType _selectedTemplate;
        private Vector2 _scrollPosition;
        private Request _registryResolveRequest;
        private AddRequest _installCoreRequest;
        private string _startupMessage;
        private string _startupError;
        private bool _registryReady;

        private enum TemplateType
        {
            Normal,
            Hybrid
        }

        private enum ProjectInstallState
        {
            NotInstalled,
            Custom,
            NormalTemplate,
            HybridTemplate
        }

        [MenuItem(MENU_PATH, false, -3000)]
        private static void OpenWindow()
        {
            FrameworkInstallerWindow window = GetWindow<FrameworkInstallerWindow>();
            window.titleContent = new GUIContent("Moirai Framework Installer", EditorGUIUtility.IconContent("Package Manager").image);
            window.minSize = new Vector2(560f, 460f);
            window.Show();
        }

        private void OnEnable()
        {
            EnsureRegistryBeforeDisplay();
        }

        private void OnDisable()
        {
            EditorApplication.update -= MonitorRegistryResolve;
            EditorApplication.update -= MonitorCoreInstall;
        }

        private void OnGUI()
        {
            if (!_registryReady)
            {
                DrawStartupPanel();
                return;
            }

            EnsureCheckResult();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawHeader();
            DrawEnvironmentPanel();
            DrawCorePanel();
            DrawTemplatePanel();
            DrawActionPanel();
            EditorGUILayout.EndScrollView();
        }

        private void EnsureRegistryBeforeDisplay()
        {
            _registryReady = false;
            _startupError = string.Empty;
            _startupMessage = "Checking OpenUPM scoped registry...";

            if (!EnsureRequiredScopedRegistry(out string error, out bool changed))
            {
                _startupError = error;
                Repaint();
                return;
            }

            if (changed)
            {
                _startupMessage = "OpenUPM scoped registry was updated. Waiting for Package Manager to resolve...";
                Client.Resolve();
                _registryResolveRequest = Client.List();
                EditorApplication.update -= MonitorRegistryResolve;
                EditorApplication.update += MonitorRegistryResolve;
                Repaint();
                return;
            }

            _registryReady = true;
            RunInstallCheck();
        }

        private void MonitorRegistryResolve()
        {
            if (_registryResolveRequest == null || !_registryResolveRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= MonitorRegistryResolve;
            if (_registryResolveRequest.Status == StatusCode.Failure)
            {
                _startupError = "Package Manager resolve failed: " + _registryResolveRequest.Error.message;
                _registryResolveRequest = null;
                Repaint();
                return;
            }

            _registryResolveRequest = null;
            _registryReady = true;
            AssetDatabase.Refresh();
            RunInstallCheck();
        }

        private void DrawStartupPanel()
        {
            EditorGUILayout.Space(8f);
            using (new InstallerGui.BoxGroupScope("Moirai Framework Installer", 26f))
            {
                if (!string.IsNullOrEmpty(_startupError))
                {
                    InstallerGui.HelpBox(_startupError, MessageType.Error);
                    if (GUILayout.Button("Retry", InstallerGui.InlineButton, GUILayout.Width(120f)))
                    {
                        EnsureRegistryBeforeDisplay();
                    }

                    return;
                }

                InstallerGui.HelpBox(_startupMessage, MessageType.Info);
            }
        }

        private void DrawHeader()
        {
            using (new InstallerGui.BoxGroupScope("Moirai Framework Installer", 26f))
            {
                EditorGUILayout.LabelField("Install Core first, then import a project template.", InstallerGui.MutedLabel);
            }
        }

        private void DrawEnvironmentPanel()
        {
            using (new InstallerGui.BoxGroupScope("Environment", 24f))
            {
                DrawStatusRow("OpenUPM registry", _checkResult.hasRequiredScopedRegistry, _checkResult.requiredScopedRegistryText);
                DrawStatusRow("Unity 2022.3 or newer", _checkResult.unityVersionSupported, _checkResult.unityVersion);
                DrawStatusRow("Core package", _checkResult.hasCorePackage, _checkResult.corePackageText);
                DrawStatusRow("URP package", _checkResult.hasUrp, _checkResult.urpVersionText, MessageType.Warning);
                DrawStatusRow("HybridCLR package", _checkResult.hasHybridClr, _checkResult.hybridClrVersionText, MessageType.Warning);
                DrawStatusRow("Installer state", _checkResult.projectState != ProjectInstallState.NotInstalled, _checkResult.StateText, MessageType.Warning);
                DrawStatusRow("State source", true, _checkResult.stateSource);
                DrawStatusRow("Normal template folder", _checkResult.hasNormalTemplate, NORMAL_TEMPLATE_PATH);
                DrawStatusRow("Hybrid template folder", _checkResult.hasHybridTemplate, HYBRID_TEMPLATE_PATH, MessageType.Warning);
            }
        }

        private void DrawCorePanel()
        {
            using (new InstallerGui.BoxGroupScope("Core", 24f))
            {
                if (_checkResult.hasCorePackage)
                {
                    InstallerGui.HelpBox("Moirai Framework is installed. Template installation is available.", MessageType.Info);
                    return;
                }

                InstallerGui.HelpBox("Install Core before importing templates. This installs " + CORE_PACKAGE_NAME + " and lets Unity resolve its dependencies.", MessageType.Warning);
                using (new EditorGUI.DisabledScope(_installCoreRequest != null))
                {
                    string label = _installCoreRequest == null ? "Install Core" : "Installing Core...";
                    if (GUILayout.Button(label, InstallerGui.InlineButton, GUILayout.Width(160f)))
                    {
                        InstallCorePackage();
                    }
                }
            }
        }

        private void DrawTemplatePanel()
        {
            using (new InstallerGui.BoxGroupScope("Template", 24f))
            {
                if (!_checkResult.hasCorePackage)
                {
                    InstallerGui.HelpBox("Install Core first. Template options are locked until " + CORE_PACKAGE_NAME + " is installed.", MessageType.Info);
                    return;
                }

                if (_checkResult.projectState == ProjectInstallState.Custom)
                {
                    InstallerGui.HelpBox("Project is marked as custom/no template required. You can still import a template later.", MessageType.Info);
                }

                if (_checkResult.projectState == ProjectInstallState.HybridTemplate)
                {
                    InstallerGui.HelpBox("Hybrid template is already initialized. Installer is locked to avoid overwriting project files.", MessageType.Info);
                    return;
                }

                if (_checkResult.projectState == ProjectInstallState.NormalTemplate)
                {
                    InstallerGui.HelpBox("Normal template is initialized. You can upgrade to Hybrid template after HybridCLR is installed.", MessageType.Info);
                    using (new EditorGUI.DisabledScope(!_checkResult.hasHybridClr || !_checkResult.hasHybridTemplate))
                    {
                        _selectedTemplate = TemplateType.Hybrid;
                        DrawTemplateChoice("Hybrid Template", true, "Upgrade current project to hot update template.");
                    }

                    return;
                }

                DrawTemplateChoice("Normal Template", _selectedTemplate == TemplateType.Normal, "Standalone framework template. Adds ENABLE_LOG.");

                using (new EditorGUI.DisabledScope(!_checkResult.hasHybridClr))
                {
                    bool selected = _selectedTemplate == TemplateType.Hybrid;
                    bool nextSelected = DrawTemplateChoice("Hybrid Template", selected, "Hot update framework template. Adds ENABLE_LOG and ENABLE_HYBRIDCLR.");
                    if (nextSelected && !selected)
                    {
                        _selectedTemplate = TemplateType.Hybrid;
                    }
                }

                if (!_checkResult.hasHybridClr)
                {
                    InstallerGui.HelpBox("Hybrid template requires HybridCLR package.", MessageType.Warning);
                }
            }
        }

        private void DrawActionPanel()
        {
            using (new InstallerGui.BoxGroupScope("Actions", 24f))
            {
                bool canInstall = CanInstallSelectedTemplate(out string blockReason);
                if (!string.IsNullOrEmpty(blockReason))
                {
                    InstallerGui.HelpBox(blockReason, MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh Check", InstallerGui.InlineButton, GUILayout.Width(130f)))
                {
                    RunInstallCheck();
                }

                if (_checkResult.projectState == ProjectInstallState.NotInstalled)
                {
                    using (new EditorGUI.DisabledScope(!_checkResult.hasCorePackage))
                    {
                        if (GUILayout.Button("Use Custom", InstallerGui.InlineButton, GUILayout.Width(120f)))
                        {
                            SaveInstallState(ProjectInstallState.Custom);
                            RunInstallCheck();
                        }
                    }
                }

                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(!canInstall))
                {
                    string label = _checkResult.projectState == ProjectInstallState.NormalTemplate ? "Upgrade Template" : "Install Template";
                    if (GUILayout.Button(label, InstallerGui.InlineButton, GUILayout.Width(180f)))
                    {
                        InstallSelectedTemplate();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private bool DrawTemplateChoice(string title, bool selected, string description)
        {
            EditorGUILayout.BeginVertical(InstallerGui.EntryBody);
            EditorGUILayout.BeginHorizontal();

            bool nextSelected = GUILayout.Toggle(selected, GUIContent.none, GUILayout.Width(18f));
            EditorGUILayout.LabelField(title, InstallerGui.RowLabel);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(description, InstallerGui.MutedMiniLabel);
            EditorGUILayout.EndVertical();

            if (nextSelected)
            {
                _selectedTemplate = title.StartsWith("Hybrid", StringComparison.Ordinal) ? TemplateType.Hybrid : TemplateType.Normal;
            }

            return nextSelected;
        }

        private void DrawStatusRow(string label, bool success, string message, MessageType failedType = MessageType.Error)
        {
            EditorGUILayout.BeginHorizontal(InstallerGui.FieldRow);
            GUIContent icon = success ? InstallerGui.GreenLight : InstallerGui.RedLight;
            GUILayout.Label(icon, GUILayout.Width(22f), GUILayout.Height(18f));
            EditorGUILayout.LabelField(label, InstallerGui.FieldLabel, GUILayout.Width(160f));
            GUIStyle valueStyle = success ? InstallerGui.RowLabel : failedType == MessageType.Warning ? InstallerGui.WarningLabel : InstallerGui.WarningLabel;
            EditorGUILayout.LabelField(message, valueStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void EnsureCheckResult()
        {
            if (_checkResult == null)
            {
                RunInstallCheck();
            }
        }

        private void RunInstallCheck()
        {
            _checkResult = InstallCheckResult.Create();

            if (_checkResult.projectState == ProjectInstallState.NotInstalled ||
                _checkResult.projectState == ProjectInstallState.Custom)
            {
                _selectedTemplate = _checkResult.hasHybridClr ? TemplateType.Hybrid : TemplateType.Normal;
            }
            else if (_checkResult.projectState == ProjectInstallState.NormalTemplate)
            {
                _selectedTemplate = TemplateType.Hybrid;
            }
            else
            {
                _selectedTemplate = TemplateType.Normal;
            }

            Repaint();
        }

        private bool CanInstallSelectedTemplate(out string blockReason)
        {
            blockReason = string.Empty;

            if (!_checkResult.hasRequiredScopedRegistry)
            {
                blockReason = "OpenUPM scoped registry is still being configured.";
                return false;
            }

            if (!_checkResult.unityVersionSupported)
            {
                blockReason = "Unity version must be 2022.3.x or newer.";
                return false;
            }

            if (!_checkResult.hasCorePackage)
            {
                blockReason = "Install Core before importing templates.";
                return false;
            }

            if (!_checkResult.hasUrp)
            {
                blockReason = "URP package is required before installing Moirai framework templates.";
                return false;
            }

            if (_checkResult.projectState == ProjectInstallState.HybridTemplate)
            {
                blockReason = "Hybrid template is already initialized.";
                return false;
            }

            if (_checkResult.projectState == ProjectInstallState.NormalTemplate && _selectedTemplate != TemplateType.Hybrid)
            {
                blockReason = "Normal template is already initialized and cannot be overwritten.";
                return false;
            }

            if (_selectedTemplate == TemplateType.Hybrid && !_checkResult.hasHybridClr)
            {
                blockReason = "Hybrid template requires HybridCLR package.";
                return false;
            }

            string templatePath = GetSelectedTemplatePath();
            if (!Directory.Exists(templatePath))
            {
                blockReason = "Template folder is missing: " + templatePath;
                return false;
            }

            return true;
        }

        private void InstallCorePackage()
        {
            if (!EnsureRequiredScopedRegistry(out string registryError, out bool changed))
            {
                EditorUtility.DisplayDialog("Moirai Framework Installer", registryError, "OK");
                EnsureRegistryBeforeDisplay();
                return;
            }

            if (changed)
            {
                EnsureRegistryBeforeDisplay();
                return;
            }

            if (_installCoreRequest != null)
            {
                return;
            }

            _installCoreRequest = Client.Add(CORE_PACKAGE_NAME);
            EditorApplication.update -= MonitorCoreInstall;
            EditorApplication.update += MonitorCoreInstall;
            Repaint();
        }

        private void MonitorCoreInstall()
        {
            if (_installCoreRequest == null || !_installCoreRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= MonitorCoreInstall;
            AddRequest request = _installCoreRequest;
            _installCoreRequest = null;

            if (request.Status == StatusCode.Failure)
            {
                EditorUtility.DisplayDialog("Moirai Framework Installer", "Failed to install Core: " + request.Error.message, "OK");
                RunInstallCheck();
                return;
            }

            AssetDatabase.Refresh();
            RunInstallCheck();
            EditorUtility.DisplayDialog("Moirai Framework Installer", "Core package installed. You can now import a template.", "OK");
        }

        private void InstallSelectedTemplate()
        {
            if (!CanInstallSelectedTemplate(out string blockReason))
            {
                EditorUtility.DisplayDialog("Moirai Framework Installer", blockReason, "OK");
                RunInstallCheck();
                return;
            }

            string templatePath = GetSelectedTemplatePath();
            string prompt = _checkResult.projectState == ProjectInstallState.NormalTemplate
                ? "Upgrade the current project to the Hybrid template?"
                : "Install the selected template into the current project?";

            if (!EditorUtility.DisplayDialog("Moirai Framework Installer", prompt + "\n\n" + templatePath, "Install", "Cancel"))
            {
                return;
            }

            if (_selectedTemplate == TemplateType.Hybrid && !ConfirmHybridPlayerSettings())
            {
                return;
            }

            CopyTemplateDirectory(templatePath);
            ApplyPlayerSettings();
            ApplyScriptingDefineSymbols(_selectedTemplate);
            SaveInstallState(_selectedTemplate == TemplateType.Hybrid ? ProjectInstallState.HybridTemplate : ProjectInstallState.NormalTemplate);
            AssetDatabase.Refresh();
            RunInstallCheck();

            EditorUtility.DisplayDialog("Moirai Framework Installer", "Template installation complete.", "OK");
        }

        private static bool EnsureRequiredScopedRegistry(out string error, out bool changed)
        {
            error = string.Empty;
            changed = false;

            if (HasScopedRegistry(REQUIRED_REGISTRY_URL, s_RequiredRegistryScopes))
            {
                return true;
            }

            try
            {
                if (!File.Exists(MANIFEST_PATH))
                {
                    error = "Package manifest is missing: " + MANIFEST_PATH;
                    return false;
                }

                string manifest = File.ReadAllText(MANIFEST_PATH);
                string nextManifest = AddOrUpdateScopedRegistry(manifest, REQUIRED_REGISTRY_NAME, REQUIRED_REGISTRY_URL, s_RequiredRegistryScopes);

                if (string.Equals(manifest, nextManifest, StringComparison.Ordinal))
                {
                    error = "Failed to update OpenUPM scoped registry in " + MANIFEST_PATH + ".";
                    return false;
                }

                File.WriteAllText(MANIFEST_PATH, nextManifest);
                changed = true;
                Debug.Log("Moirai Framework Installer updated OpenUPM scoped registry in " + MANIFEST_PATH + ".");
                return true;
            }
            catch (Exception ex)
            {
                error = "Failed to update Package Manager scoped registry: " + ex.Message;
                return false;
            }
        }

        private static string AddOrUpdateScopedRegistry(string manifest, string registryName, string registryUrl, params string[] requiredScopes)
        {
            Match registryMatch = Regex.Match(
                manifest,
                "\\{\\s*\"name\"\\s*:\\s*\"[^\"]*\"\\s*,\\s*\"url\"\\s*:\\s*\"" + Regex.Escape(registryUrl) + "\"\\s*,\\s*\"scopes\"\\s*:\\s*\\[(?<scopes>[\\s\\S]*?)\\]\\s*\\}",
                RegexOptions.Singleline);

            if (registryMatch.Success)
            {
                string scopeBlock = registryMatch.Groups["scopes"].Value;
                string registryJson = BuildScopedRegistryJson(
                    registryName,
                    registryUrl,
                    MergeScopes(scopeBlock, requiredScopes),
                    GetRegistryIndent(manifest, registryMatch.Index));

                return manifest.Substring(0, registryMatch.Index) +
                       registryJson +
                       manifest.Substring(registryMatch.Index + registryMatch.Length);
            }

            string newRegistryJson = BuildScopedRegistryJson(registryName, registryUrl, requiredScopes, "    ");
            Match scopedRegistriesMatch = Regex.Match(manifest, "\"scopedRegistries\"\\s*:\\s*\\[(?<content>[\\s\\S]*?)\\]\\s*(?=\\n\\s*\\})", RegexOptions.Singleline);
            if (scopedRegistriesMatch.Success)
            {
                string content = scopedRegistriesMatch.Groups["content"].Value;
                string nextContent = string.IsNullOrWhiteSpace(content)
                    ? "\n" + newRegistryJson + "\n  "
                    : content.TrimEnd() + ",\n" + newRegistryJson + "\n  ";

                return manifest.Substring(0, scopedRegistriesMatch.Groups["content"].Index) +
                       nextContent +
                       manifest.Substring(scopedRegistriesMatch.Groups["content"].Index + scopedRegistriesMatch.Groups["content"].Length);
            }

            int insertIndex = manifest.LastIndexOf('}');
            if (insertIndex < 0)
            {
                return manifest;
            }

            string prefix = manifest.Substring(0, insertIndex).TrimEnd();
            string suffix = manifest.Substring(insertIndex);
            string separator = prefix.EndsWith("{", StringComparison.Ordinal) ? "\n" : ",\n";
            return prefix + separator + "  \"scopedRegistries\": [\n" + newRegistryJson + "\n  ]\n" + suffix;
        }

        private static string[] MergeScopes(string scopeBlock, string[] requiredScopes)
        {
            string[] existingScopes = Regex.Matches(scopeBlock, "\"(?<scope>[^\"]+)\"")
                .Cast<Match>()
                .Select(match => match.Groups["scope"].Value)
                .ToArray();

            string[] mergedScopes = new string[existingScopes.Length + requiredScopes.Length];
            int count = 0;

            foreach (string scope in existingScopes)
            {
                if (ContainsScope(mergedScopes, count, scope))
                {
                    continue;
                }

                mergedScopes[count++] = scope;
            }

            foreach (string scope in requiredScopes)
            {
                if (ContainsScope(mergedScopes, count, scope))
                {
                    continue;
                }

                mergedScopes[count++] = scope;
            }

            Array.Resize(ref mergedScopes, count);
            return mergedScopes;
        }

        private static bool ContainsScope(string[] scopes, int count, string scope)
        {
            for (int i = 0; i < count; i++)
            {
                if (string.Equals(scopes[i], scope, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetRegistryIndent(string manifest, int registryIndex)
        {
            int lineStart = manifest.LastIndexOf('\n', Math.Max(0, registryIndex - 1));
            if (lineStart < 0)
            {
                return "    ";
            }

            int indentStart = lineStart + 1;
            int indentLength = 0;
            while (indentStart + indentLength < manifest.Length && char.IsWhiteSpace(manifest[indentStart + indentLength]))
            {
                indentLength++;
            }

            return indentLength > 0 ? manifest.Substring(indentStart, indentLength) : "    ";
        }

        private static string BuildScopedRegistryJson(string registryName, string registryUrl, string[] scopes, string indent)
        {
            string scopeIndent = indent + "    ";
            string json = indent + "{\n" +
                          indent + "  \"name\": \"" + registryName + "\",\n" +
                          indent + "  \"url\": \"" + registryUrl + "\",\n" +
                          indent + "  \"scopes\": [";

            for (int i = 0; i < scopes.Length; i++)
            {
                json += "\n" + scopeIndent + "\"" + scopes[i] + "\"";
                if (i < scopes.Length - 1)
                {
                    json += ",";
                }
            }

            return json + "\n" + indent + "  ]\n" + indent + "}";
        }

        private string GetSelectedTemplatePath()
        {
            return _selectedTemplate == TemplateType.Hybrid ? HYBRID_TEMPLATE_PATH : NORMAL_TEMPLATE_PATH;
        }

        private static void CopyTemplateDirectory(string templatePath)
        {
            string sourceRoot = Path.GetFullPath(templatePath);
            string targetRoot = Application.dataPath;

            foreach (string sourceDirectory in Directory.GetDirectories(sourceRoot, "*", SearchOption.AllDirectories))
            {
                string relativeDirectory = GetRelativePath(sourceRoot, sourceDirectory);
                Directory.CreateDirectory(Path.Combine(targetRoot, relativeDirectory));
            }

            foreach (string sourceFile in Directory.GetFiles(sourceRoot, "*", SearchOption.AllDirectories))
            {
                if (IsTemplatePlaceholder(sourceFile))
                {
                    continue;
                }

                string relativeFile = GetRelativePath(sourceRoot, sourceFile);
                string targetFile = Path.Combine(targetRoot, relativeFile);
                string targetDirectory = Path.GetDirectoryName(targetFile);
                if (!string.IsNullOrEmpty(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                if (File.Exists(targetFile))
                {
                    Debug.LogWarning("Moirai Framework Installer skipped existing file: " + relativeFile);
                    continue;
                }

                File.Copy(sourceFile, targetFile);
            }
        }

        private static bool IsTemplatePlaceholder(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            if (string.Equals(fileName, ".keep", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileName, ".gitkeep", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string fileNameWithoutMeta = fileName;
            if (fileName.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
            {
                fileNameWithoutMeta = Path.GetFileNameWithoutExtension(fileName);
            }

            return string.Equals(fileNameWithoutMeta, ".keep", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fileNameWithoutMeta, ".gitkeep", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetRelativePath(string rootPath, string path)
        {
            Uri rootUri = new Uri(AppendDirectorySeparatorChar(rootPath));
            Uri pathUri = new Uri(path);
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(pathUri).ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? path
                : path + Path.DirectorySeparatorChar;
        }

        private static void ApplyScriptingDefineSymbols(TemplateType templateType)
        {
            ScriptingDefineSymbolUtility.AddScriptingDefineSymbol(ENABLE_LOG_SYMBOL);

            if (templateType == TemplateType.Hybrid)
            {
                ScriptingDefineSymbolUtility.AddScriptingDefineSymbol(ENABLE_HYBRID_CLR_SYMBOL);
            }
        }

        private static void ApplyPlayerSettings()
        {
            if (!PlayerSettings.allowUnsafeCode)
            {
                PlayerSettings.allowUnsafeCode = true;
            }
        }

        private static bool ConfirmHybridPlayerSettings()
        {
            int option = EditorUtility.DisplayDialogComplex(
                "Moirai Framework Installer",
                "Installing the Hybrid template requires these Player Settings:\n\n" +
                "1. Scripting Backend: IL2CPP.\n" +
                "2. Api Compatibility Level: .NET Framework.\n" +
                "3. Incremental GC: disabled.\n\n" +
                "Apply these settings for the current platform?",
                "Apply",
                "Manual",
                "Cancel");

            if (option == 2)
            {
                return false;
            }

            if (option == 0)
            {
                ApplyHybridPlayerSettings();
            }

            return true;
        }

        private static void ApplyHybridPlayerSettings()
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
#if UNITY_6000_0_OR_NEWER
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            PlayerSettings.SetScriptingBackend(namedBuildTarget, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(namedBuildTarget, ApiCompatibilityLevel.NET_4_6);
#else
            PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(buildTargetGroup, ApiCompatibilityLevel.NET_4_6);
#endif
            PlayerSettings.gcIncremental = false;
        }

        private static void SaveInstallState(ProjectInstallState state)
        {
            string json = JsonUtility.ToJson(new InstallStateData
            {
                installerState = state.ToString(),
                template = ToTemplateText(state),
                unityVersion = Application.unityVersion,
                projectPath = Path.GetFullPath("."),
                updatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }, true);

            File.WriteAllText(INSTALL_STATE_PATH, json);
        }

        private static ProjectInstallState ReadInstallState(out string source)
        {
            if (File.Exists(INSTALL_STATE_PATH))
            {
                try
                {
                    InstallStateData data = JsonUtility.FromJson<InstallStateData>(File.ReadAllText(INSTALL_STATE_PATH));
                    if (TryParseState(data.installerState, out ProjectInstallState fileState))
                    {
                        ProjectInstallState validatedState = ValidatePersistedInstallState(fileState);
                        source = validatedState == fileState ? INSTALL_STATE_PATH : INSTALL_STATE_PATH + " (missing template assets)";
                        return validatedState;
                    }

                    if (!string.IsNullOrEmpty(data.template) && TryParseLegacyTemplate(data.template, out fileState))
                    {
                        ProjectInstallState validatedState = ValidatePersistedInstallState(fileState);
                        source = validatedState == fileState ? INSTALL_STATE_PATH + " (legacy)" : INSTALL_STATE_PATH + " (legacy, missing template assets)";
                        return validatedState;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Failed to read Moirai Framework Installer state: " + ex.Message);
                }
            }

            bool hasLogSymbol = ScriptingDefineSymbolUtility.HasScriptingDefineSymbol(EditorUserBuildSettings.selectedBuildTargetGroup, ENABLE_LOG_SYMBOL);
            bool hasHybridSymbol = ScriptingDefineSymbolUtility.HasScriptingDefineSymbol(EditorUserBuildSettings.selectedBuildTargetGroup, ENABLE_HYBRID_CLR_SYMBOL);

            if (hasHybridSymbol || HasHybridAssetMarkers())
            {
                source = "Compatibility fallback";
                return ProjectInstallState.HybridTemplate;
            }

            if (hasLogSymbol || HasRuntimeAssetMarkers())
            {
                source = "Compatibility fallback";
                return ProjectInstallState.NormalTemplate;
            }

            source = "Default";
            return ProjectInstallState.NotInstalled;
        }

        private static ProjectInstallState ValidatePersistedInstallState(ProjectInstallState state)
        {
            if (state == ProjectInstallState.NormalTemplate && !HasRuntimeAssetMarkers())
            {
                return ProjectInstallState.NotInstalled;
            }

            if (state == ProjectInstallState.HybridTemplate)
            {
                if (HasHybridAssetMarkers())
                {
                    return ProjectInstallState.HybridTemplate;
                }

                return HasRuntimeAssetMarkers() ? ProjectInstallState.NormalTemplate : ProjectInstallState.NotInstalled;
            }

            return state;
        }

        private static bool TryParseState(string value, out ProjectInstallState state)
        {
            if (string.IsNullOrEmpty(value))
            {
                state = ProjectInstallState.NotInstalled;
                return false;
            }

            if (Enum.TryParse(value, true, out state))
            {
                return true;
            }

            state = ProjectInstallState.NotInstalled;
            return false;
        }

        private static bool TryParseLegacyTemplate(string value, out ProjectInstallState state)
        {
            if (string.Equals(value, "Normal", StringComparison.OrdinalIgnoreCase))
            {
                state = ProjectInstallState.NormalTemplate;
                return true;
            }

            if (string.Equals(value, "Hybrid", StringComparison.OrdinalIgnoreCase))
            {
                state = ProjectInstallState.HybridTemplate;
                return true;
            }

            return TryParseState(value, out state);
        }

        private static string ToTemplateText(ProjectInstallState state)
        {
            if (state == ProjectInstallState.NormalTemplate)
            {
                return "Normal";
            }

            if (state == ProjectInstallState.HybridTemplate)
            {
                return "Hybrid";
            }

            if (state == ProjectInstallState.Custom)
            {
                return "Custom";
            }

            return string.Empty;
        }

        private static bool HasRuntimeAssetMarkers()
        {
            foreach (string marker in s_RuntimeAssetMarkers)
            {
                if (AssetDatabase.IsValidFolder(marker) || File.Exists(marker))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasHybridAssetMarkers()
        {
            foreach (string marker in s_HybridAssetMarkers)
            {
                if (AssetDatabase.IsValidFolder(marker) || File.Exists(marker))
                {
                    return true;
                }
            }

            return false;
        }

        private static string FindManifestDependencyVersion(string packageName)
        {
            if (string.IsNullOrEmpty(packageName) || !File.Exists(MANIFEST_PATH))
            {
                return string.Empty;
            }

            string manifest = File.ReadAllText(MANIFEST_PATH);
            Match match = Regex.Match(manifest, "\"" + Regex.Escape(packageName) + "\"\\s*:\\s*\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static bool HasScopedRegistry(string url, params string[] scopes)
        {
            if (string.IsNullOrEmpty(url) || scopes == null || scopes.Length == 0 || !File.Exists(MANIFEST_PATH))
            {
                return false;
            }

            try
            {
                ManifestData manifestData = JsonUtility.FromJson<ManifestData>(File.ReadAllText(MANIFEST_PATH));
                if (manifestData?.scopedRegistries == null)
                {
                    return false;
                }

                foreach (ScopedRegistryData registry in manifestData.scopedRegistries)
                {
                    if (registry == null ||
                        !string.Equals(NormalizeUrl(registry.url), NormalizeUrl(url), StringComparison.OrdinalIgnoreCase) ||
                        registry.scopes == null)
                    {
                        continue;
                    }

                    bool containsAllScopes = true;
                    foreach (string scope in scopes)
                    {
                        if (!Array.Exists(registry.scopes, registryScope => string.Equals(registryScope, scope, StringComparison.Ordinal)))
                        {
                            containsAllScopes = false;
                            break;
                        }
                    }

                    if (containsAllScopes)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Failed to read Package Manager scoped registries: " + ex.Message);
            }

            return false;
        }

        private static string NormalizeUrl(string url)
        {
            return string.IsNullOrEmpty(url) ? string.Empty : url.Trim().TrimEnd('/');
        }

        private sealed class InstallCheckResult
        {
            public ProjectInstallState projectState;
            public string stateSource;
            public bool unityVersionSupported;
            public string unityVersion;
            public bool hasRequiredScopedRegistry;
            public string requiredScopedRegistryText;
            public bool hasCorePackage;
            public string corePackageText;
            public bool hasUrp;
            public string urpVersionText;
            public bool hasHybridClr;
            public string hybridClrVersionText;
            public bool hasNormalTemplate;
            public bool hasHybridTemplate;

            public string StateText
            {
                get
                {
                    switch (projectState)
                    {
                        case ProjectInstallState.Custom:
                            return "Custom / no template required";
                        case ProjectInstallState.NormalTemplate:
                            return "Normal template";
                        case ProjectInstallState.HybridTemplate:
                            return "Hybrid template";
                        default:
                            return "Not initialized";
                    }
                }
            }

            public static InstallCheckResult Create()
            {
                string corePackageVersion = FindCorePackageVersion();
                string urpVersion = FindManifestDependencyVersion(URP_PACKAGE_NAME);
                string hybridClrVersion = FindManifestDependencyVersion(HYBRID_CLR_PACKAGE_NAME);
                bool hasRequiredScopedRegistry = HasScopedRegistry(REQUIRED_REGISTRY_URL, s_RequiredRegistryScopes);
                ProjectInstallState projectState = ReadInstallState(out string stateSource);

                return new InstallCheckResult
                {
                    projectState = projectState,
                    stateSource = stateSource,
                    unityVersionSupported = IsUnityVersionSupported(Application.unityVersion),
                    unityVersion = Application.unityVersion,
                    hasRequiredScopedRegistry = hasRequiredScopedRegistry,
                    requiredScopedRegistryText = hasRequiredScopedRegistry
                        ? REQUIRED_REGISTRY_URL + " (" + string.Join(", ", s_RequiredRegistryScopes) + ")"
                        : "Missing " + REQUIRED_REGISTRY_URL + " scopes: " + string.Join(", ", s_RequiredRegistryScopes),
                    hasCorePackage = !string.IsNullOrEmpty(corePackageVersion),
                    corePackageText = string.IsNullOrEmpty(corePackageVersion) ? "Not installed" : corePackageVersion,
                    hasUrp = !string.IsNullOrEmpty(urpVersion),
                    urpVersionText = string.IsNullOrEmpty(urpVersion) ? "Not installed" : urpVersion,
                    hasHybridClr = !string.IsNullOrEmpty(hybridClrVersion),
                    hybridClrVersionText = string.IsNullOrEmpty(hybridClrVersion) ? "Not installed" : hybridClrVersion,
                    hasNormalTemplate = Directory.Exists(NORMAL_TEMPLATE_PATH),
                    hasHybridTemplate = Directory.Exists(HYBRID_TEMPLATE_PATH)
                };
            }

            private static string FindCorePackageVersion()
            {
                string manifestVersion = FindManifestDependencyVersion(CORE_PACKAGE_NAME);
                if (!string.IsNullOrEmpty(manifestVersion))
                {
                    return manifestVersion;
                }

                string embeddedPackageJson = "Packages/" + CORE_PACKAGE_NAME + "/package.json";
                if (File.Exists(embeddedPackageJson))
                {
                    string packageJson = File.ReadAllText(embeddedPackageJson);
                    Match match = Regex.Match(packageJson, "\"version\"\\s*:\\s*\"([^\"]+)\"");
                    return match.Success ? "Embedded " + match.Groups[1].Value : "Embedded package";
                }

                return string.Empty;
            }

            private static bool IsUnityVersionSupported(string version)
            {
                if (string.IsNullOrEmpty(version))
                {
                    return false;
                }

                string[] parts = version.Split('.');
                if (parts.Length < 2)
                {
                    return false;
                }

                if (!int.TryParse(parts[0], out int major) || !int.TryParse(parts[1], out int minor))
                {
                    return false;
                }

                return major > 2022 || major == 2022 && minor >= 3;
            }
        }

        [Serializable]
        private sealed class InstallStateData
        {
            public string installerState;
            public string template;
            public string unityVersion;
            public string projectPath;
            public string updatedAt;
        }

        [Serializable]
        private sealed class ManifestData
        {
            public ScopedRegistryData[] scopedRegistries;
        }

        [Serializable]
        private sealed class ScopedRegistryData
        {
            public string name;
            public string url;
            public string[] scopes;
        }
    }

    internal static class ScriptingDefineSymbolUtility
    {
        private static readonly BuildTargetGroup[] s_BuildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.iOS,
            BuildTargetGroup.Android,
            BuildTargetGroup.WSA,
            BuildTargetGroup.WebGL
        };

        public static bool HasScriptingDefineSymbol(BuildTargetGroup buildTargetGroup, string scriptingDefineSymbol)
        {
            if (string.IsNullOrEmpty(scriptingDefineSymbol))
            {
                return false;
            }

            string[] scriptingDefineSymbols = GetScriptingDefineSymbols(buildTargetGroup);
            foreach (string symbol in scriptingDefineSymbols)
            {
                if (symbol == scriptingDefineSymbol)
                {
                    return true;
                }
            }

            return false;
        }

        public static void AddScriptingDefineSymbol(string scriptingDefineSymbol)
        {
            if (string.IsNullOrEmpty(scriptingDefineSymbol))
            {
                return;
            }

            foreach (BuildTargetGroup buildTargetGroup in s_BuildTargetGroups)
            {
                AddScriptingDefineSymbol(buildTargetGroup, scriptingDefineSymbol);
            }
        }

        private static void AddScriptingDefineSymbol(BuildTargetGroup buildTargetGroup, string scriptingDefineSymbol)
        {
            if (HasScriptingDefineSymbol(buildTargetGroup, scriptingDefineSymbol))
            {
                return;
            }

            string[] currentSymbols = GetScriptingDefineSymbols(buildTargetGroup);
            string[] nextSymbols = new string[currentSymbols.Length + 1];
            Array.Copy(currentSymbols, nextSymbols, currentSymbols.Length);
            nextSymbols[nextSymbols.Length - 1] = scriptingDefineSymbol;
            SetScriptingDefineSymbols(buildTargetGroup, nextSymbols);
        }

        private static string[] GetScriptingDefineSymbols(BuildTargetGroup buildTargetGroup)
        {
#if UNITY_6000_0_OR_NEWER
            return PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup)).Split(';');
#else
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';');
#endif
        }

        private static void SetScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string[] scriptingDefineSymbols)
        {
#if UNITY_6000_0_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup), string.Join(";", scriptingDefineSymbols));
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", scriptingDefineSymbols));
#endif
        }
    }

    internal static class InstallerGui
    {
        private const float DEFAULT_CONTROL_HEIGHT = 20f;
        private static GUIStyle s_InlineButton;
        private static GUIStyle s_EntryBody;
        private static GUIStyle s_FieldRow;
        private static GUIStyle s_RowLabel;
        private static GUIStyle s_MutedLabel;
        private static GUIStyle s_FieldLabel;
        private static GUIStyle s_MutedMiniLabel;
        private static GUIStyle s_WarningLabel;

        public static GUIContent GreenLight => EditorGUIUtility.TrIconContent("greenLight");
        public static GUIContent RedLight => EditorGUIUtility.TrIconContent("redLight");

        public static GUIStyle InlineButton
        {
            get
            {
                if (s_InlineButton == null)
                {
                    s_InlineButton = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fixedHeight = DEFAULT_CONTROL_HEIGHT,
                        padding = new RectOffset(6, 6, 1, 1)
                    };
                }

                return s_InlineButton;
            }
        }

        public static GUIStyle EntryBody
        {
            get
            {
                if (s_EntryBody == null)
                {
                    s_EntryBody = new GUIStyle(EditorStyles.helpBox)
                    {
                        padding = new RectOffset(8, 8, 6, 8),
                        margin = new RectOffset(0, 0, 0, 4)
                    };
                }

                return s_EntryBody;
            }
        }

        public static GUIStyle FieldRow
        {
            get
            {
                if (s_FieldRow == null)
                {
                    s_FieldRow = new GUIStyle(EditorStyles.helpBox)
                    {
                        padding = new RectOffset(5, 5, 3, 3),
                        margin = new RectOffset(0, 0, 1, 1)
                    };
                }

                return s_FieldRow;
            }
        }

        public static GUIStyle RowLabel => s_RowLabel ?? (s_RowLabel = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Clip });
        public static GUIStyle MutedLabel => s_MutedLabel ?? (s_MutedLabel = new GUIStyle(EditorStyles.label) { normal = { textColor = Color.gray }, alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Clip });
        public static GUIStyle FieldLabel => s_FieldLabel ?? (s_FieldLabel = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.gray }, alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Clip });
        public static GUIStyle MutedMiniLabel => s_MutedMiniLabel ?? (s_MutedMiniLabel = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.gray }, alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Clip });
        public static GUIStyle WarningLabel => s_WarningLabel ?? (s_WarningLabel = new GUIStyle(EditorStyles.label) { normal = { textColor = new Color(1f, 0.66f, 0.24f, 1f) }, alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Clip });

        public static void HelpBox(string message, MessageType messageType)
        {
            EditorGUILayout.HelpBox(message, messageType);
        }

        public sealed class BoxGroupScope : GUI.Scope
        {
            public BoxGroupScope(string title, float height = 22f)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                Rect headerRect = GUILayoutUtility.GetRect(1, height);
                EditorGUI.DrawRect(headerRect, new Color(0.1f, 0.1f, 0.1f, 0.4f));
                headerRect.x += EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(headerRect, title, EditorStyles.boldLabel);
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif