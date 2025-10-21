using UnityEditor;
using UnityEngine;

public static class BuildApp
{
    // GitHub Actionsから実行されるメソッド
    public static void Build()
    {
        // 🚨 macOS向けにビルドターゲットを修正
        string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        string outputPath = "Builds/App/SampleApp.app"; // macOSのアプリパス

        // macOS Standaloneでビルドする例
        BuildPipeline.BuildPlayer(
            scenes,
            outputPath,
            BuildTarget.StandaloneOSX, // ビルドターゲットをmacOSに変更
            BuildOptions.None
        );
    }
}