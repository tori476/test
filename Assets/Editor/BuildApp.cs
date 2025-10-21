using UnityEditor;

public static class BuildApp 
{
    [MenuItem("Build/BuildApp")]
    public static void Build()
    {
        //windows64のプラットフォームでアプリをビルドする
        BuildPipeline.BuildPlayer(
            new string[] { "Assets/Scenes/SampleScene.unity" },
            "Builds/App/SampleApp.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );
    }
}
