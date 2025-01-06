using Godot;

public partial class GameSystem : Node {
    public int level;
    public const int hp = 5;
    [Signal]
    public delegate void FruitCollectedEventHandler(int score);
    [Signal]
    public delegate void GameStartedEventHandler();
    [Signal]
    public delegate void GameWonEventHandler();
    [Signal]
    public delegate void GameLostEventHandler();
    [Signal]
    public delegate void HeartLostEventHandler(int currentHP);
    [Signal]
    public delegate void PlayAudioEventHandler(string name);
    [Signal]
    public delegate void PlayAudioLoopEventHandler(string name, ulong sawId);
    [Signal]
    public delegate void SetReverbEventHandler(float val);
    [Signal]
    public delegate void SetDistanceEventHandler(ulong sawId, float dis);
    [Signal]
    public delegate void StopAudioEventHandler();
}