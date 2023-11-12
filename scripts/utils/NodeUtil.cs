using System;
using Enum;
using Godot;

public partial class NodeUtil : Node {
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
        GetTree().ChangeSceneToFile(sceneName);
        StaticUtil.Log("Loading " + sceneName);
    }
    public void LoadLevel (int level) {
        var sceneName = AssetPath.SCENES + "Level" + level + ".tscn";
        GetTree().ChangeSceneToFile(sceneName);
        StaticUtil.Log("Loading " + sceneName);
    }
}