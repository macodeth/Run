using Godot;

public partial class GameSystem : Node {
    [Signal]
    public delegate void FruitCollectedEventHandler(int score);
    [Signal]
    public delegate void GameStartedEventHandler();
    [Signal]
    public delegate void GameEndedEventHandler();
}