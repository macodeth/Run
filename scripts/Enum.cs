namespace Enum;

public enum SceneId {
    LOADING,
    MAIN
}

public class AssetPath {
    public const string SCENES = "res://scenes/";
    public const string TEXTURES = "res://textures/";
}

public class AutoLoad {
    public const string NODE_UTIL = "/root/NodeUtil/";
    public const string INPUT_SYSTEM = "/root/InputSystem/";
}

public enum MoveDirection {
    LEFT,
    RIGHT,
}

public enum EventType {
    JUMP_FINISHED
}