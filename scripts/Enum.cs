namespace Enum;

public enum SceneId {
    LOADING,
    MAIN,
    SELECTION
}

public class AssetPath {
    public const string SCENES = "res://scenes/";
    public const string TEXTURES = "res://textures/";
    public const string SHADERS = "res://shaders/";
    public const string ENTITIES = "res://entities/";
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

public class GroupName {
    public const string PLAYER = "Player";
    public const string TOP_BREAKABLE = "TopBreakable";
    public const string FRUIT = "Fruit";
}

public enum LayerId {
    BACKGROUND = -1,
    DEFAULT = 0,
    UI = 1
}