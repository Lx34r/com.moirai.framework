using System;
using System.Reflection;
using Luban;
using Moirai.Atropos;
using SimpleJSON;
using UnityEngine;

namespace Moirai.GameProto.ConfigTable
{
	/// <summary>
	/// 配置加载器。桥接 Luban 生成代码与 MoiraiFramework YooAsset 资源系统。
	/// </summary>
	public partial class ConfigTableModule
	{
		private static ConfigTableModule s_Instance;
		public static ConfigTableModule Instance => s_Instance ??= new ConfigTableModule();

		private const string CONFIG_PATH = "Assets/AssetRaw/Default/Config/Table/";
		private bool _init = false;

		private Tables _tables;
		/// <summary>
		/// 所有配置表。
		/// </summary>
		public Tables Tables
		{
			get
			{
				if (!_init)
				{
					Load();
				}

				return _tables;
			}
		}

		/// <summary>
		/// 加载配置。
		/// <remarks>自动判断加载bin或json配置</remarks>
		/// </summary>
		private void Load()
		{
			ConstructorInfo tablesCtor = typeof(Tables).GetConstructors()[0];
			Type loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];
			// 根据 Tables 的构造函数的 Loader 的返回值类型决定使用 json 还是 ByteBuf Loader
			System.Delegate loader = loaderReturnType == typeof(ByteBuf)
				? new System.Func<string, ByteBuf>(LoadByteBuf)
				: (System.Delegate)new System.Func<string, JSONNode>(LoadJson);

			_tables = (Tables)tablesCtor.Invoke(new object[] { loader });
			_init = true;
		}

		/// <summary>
		/// 加载二进制配置。
		/// </summary>
		/// <param name="file">FileName</param>
		/// <returns>ByteBuf</returns>
		private static ByteBuf LoadByteBuf(string file)
		{
			Log.Info($"Load bin config: {file}.");
			TextAsset textAsset = LoadTextAsset(CONFIG_PATH + file + ".bytes");
			byte[] bytes = textAsset.bytes;
			return new ByteBuf(bytes);
		}

		/// <summary>
		/// 从文件中加载 json 配置。
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private static JSONNode LoadJson(string file)
		{
			Log.Info($"Load json config: {file}.");
			TextAsset textAsset = LoadTextAsset(CONFIG_PATH + file + ".json");
			string json = textAsset.text;
			return JSON.Parse(json);
		}

		/// <summary>
		/// 加载配置文本资源。
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		private static TextAsset LoadTextAsset(string location)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(location);
			}
#endif
			// 因为配置是预加载（Asset tag 为 PRELOAD），所以无需异步加载
			return GameModule.Resource.LoadAsset<TextAsset>(location);
		}
	}
}