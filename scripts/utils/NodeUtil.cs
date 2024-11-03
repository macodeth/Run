using System;
using Enum;
using Godot;

public partial class NodeUtil : Node {
    public override void _Ready()
    {
        // preload scene, then cache
    }
    public void LoadScene (SceneId sceneId) {
        var sceneName = "";
        switch (sceneId) {
            case SceneId.LOADING:
                sceneName = "Loading";
                break;
            case SceneId.MAIN:
                sceneName = "Main";
                break;
            case SceneId.SELECTION:
                sceneName = "Selection";
                break;
        }
        sceneName = AssetPath.SCENES + sceneName + ".tscn";
        LoadSceneFromFile(sceneName);
        StaticUtil.Log("Loading " + sceneName);
    }
    public void LoadLevel (int level) {
        var sceneName = AssetPath.SCENES + "Level" + level + ".tscn";
        LoadSceneFromFile(sceneName);
        StaticUtil.Log("Loading " + sceneName);
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.Level = level;
    }
    private void LoadSceneFromFile (string filePath) {
        var currentFilePath = GetTree().CurrentScene.SceneFilePath;
        if (filePath == currentFilePath)
            GetTree().ReloadCurrentScene();
        else
            GetTree().ChangeSceneToFile(filePath);
    }
}