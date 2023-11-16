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
    public const string POPUP = "res://popups/";
}

public class AutoLoad {
    public const string NODE_UTIL = "/root/NodeUtil/";
    public const string INPUT_SYSTEM = "/root/InputSystem/";
    public const string GAME_SYSTEM = "/root/GameSystem";
}

public enum MoveDirection {
    LEFT,
    RIGHT,
}

public enum EventType {
    ON_FLOOR,
    FALLING
}

public class GroupName {
    public const string PLAYER = "Player";
    public const string TOP_BREAKABLE = "TopBreakable";
    public const string FRUIT = "Fruit";
    public const string CONTACT_DAMAGE = "ContactDamage";
}

public enum LayerId {
    BACKGROUND = -1,
    DEFAULT = 0,
    UI = 1
}

public enum PopupType {
    RESULT
}