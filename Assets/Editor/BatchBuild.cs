#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BatchBuild
{

	private static BuildTarget m_TargetPlatform;
	private static bool m_IsRelease = false;

	// ***** Android KeyStore Settings *****//
	private static string keystorePath = "keyStoreの保存場所";
	private static string keystorePass = "ｋeyStoreのPass";
	private static string keyaliasName = "keyAliasの名前";
	private static string keyaliasPass = "keyAliasのPass";
	// ***** End Android KeyStore Settings *****//

	public static void Build()
	{
		GetCommandLineArgs();
		if (m_TargetPlatform == BuildTarget.iOS)
		{
			bool status = BuildiOS(m_IsRelease);
			EditorApplication.Exit(status ? 0 : 1);
		}
		else
		{
			bool status = BuildAndroid(m_IsRelease);
			EditorApplication.Exit(status ? 0 : 1);
		}
	}

	public static bool BuildiOS(bool isRelease = false)
	{
		Debug.Log("[ScriptLog] Start Build iOS");

		// リリースビルドではない場合Profiler等に繋げるようにする
		BuildOptions opt = BuildOptions.SymlinkLibraries;
		if (isRelease == false)
		{
			opt |= BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
		}

		string[] scenes = GetEnabledScenes();
		var errorMsg = BuildPipeline.BuildPlayer(scenes, "成果物保存場所(Assetsからの相対パス)", m_TargetPlatform, opt);

		if (errorMsg == null)
		{
			Debug.Log("[ScriptLog] Success Build iOS");
			return true;
		}

		Debug.Log("[ScriptLog] Failed Build iOS");
		Debug.Log(System.Environment.NewLine + errorMsg + System.Environment.NewLine);
		return false;
	}

	public static bool BuildAndroid(bool isRelease = false)
	{
		Debug.Log("[ScriptLog] Start Build Android");

		// リリースビルドではない場合Profiler等に繋げるようにする
		BuildOptions opt = BuildOptions.None;
		if (isRelease == false)
		{
			opt |= BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
		}

		// KeyStoreの設定
		SetAndroidKeyStoreSetting();

		string[] scenes = GetEnabledScenes();
		var errorMsg = BuildPipeline.BuildPlayer(scenes, "成果物保存場所(Assetsからの相対パス)", m_TargetPlatform, opt);

		if (errorMsg == null)
		{
			Debug.Log("[ScriptLog] Success Build Android");
			return true;
		}

		Debug.Log("[ScriptLog] Failed Build Android");
		Debug.Log(System.Environment.NewLine + errorMsg + System.Environment.NewLine);
		return false;
	}

	/// <summary>
	/// AndroidのKeyStoreの情報をPlayerSettingsに流す
	/// </summary>
	private static void SetAndroidKeyStoreSetting()
	{
		string keystoreName = System.IO.Directory.GetCurrentDirectory() + keystorePath;
		// KeyStoreの場所とパスワードを設定
		PlayerSettings.Android.keystoreName = keystoreName;
		PlayerSettings.Android.keystorePass = keystorePass;
		// KeyAliasの名前とパスワードを設定
		PlayerSettings.Android.keyaliasName = keyaliasName;
		PlayerSettings.Android.keyaliasPass = keyaliasPass;
	}


	/// <summary>
	/// コマンドライン引数を解釈して変数に格納する
	/// </summary>
	private static void GetCommandLineArgs()
	{
		string[] args = System.Environment.GetCommandLineArgs();
		// ここで引数を判定
		for (int i = 0, max = args.Length; i < max; ++i)
		{
			switch (args[i])
			{
				case "-platform":
					m_TargetPlatform = (BuildTarget)System.Enum.Parse(typeof(BuildTarget), args[i + 1]);
					break;
				case "-isRelease":
					m_IsRelease = bool.Parse(args[i + 1]);
					break;
				default:
					break;
			}
		}
	}

	/// <summary>
	/// BuildSettingで有効になっているSceneを取得
	/// </summary>
	/// <returns>The enabled scenes.</returns>
	private static string[] GetEnabledScenes()
	{
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

		List<string> sceneList = new List<string>();

		for (int i = 0, max = scenes.Length; i < max; ++i)
		{
			if (scenes[i].enabled)
			{
				sceneList.Add(scenes[i].path);
			}
		}

		return sceneList.ToArray();
	}
}
#endif