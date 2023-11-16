using Godot;

public partial class GameSystem : Node {
    public int Level;
    public const int HP = 5;
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
}