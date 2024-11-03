using Enum;
using Godot;
using System;

public partial class Level : BaseScene
{
    private int _score = 0;
    private DateTime _start_time;
	public override void _Ready()
	{
        base._Ready();
		AddBackground();
		AddHUD();
        AddEventHandlers();
	}

	private void AddBackground () {
        // add static background

        var bkg = new TextureRect
        {
            Texture = ResourceLoader.Load(AssetPath.TEXTURES + "Background/Brown.png") as Texture2D,
			LayoutMode = 1,
            AnchorsPreset = (int)LayoutPreset.FullRect,
            StretchMode = TextureRect.StretchModeEnum.Tile
        };
        var mat = new ShaderMaterial
        {
            Shader = ResourceLoader.Load(AssetPath.SHADERS + "AnimatedBkg.gdshader") as Shader
        };
        bkg.Material = mat;
        GetCanvasLayer(LayerId.BACKGROUND).AddChild(bkg);
	}
    private TimeHUD _timeHUD;
    private HeartHUD _heartHUD;
	private void AddHUD () {
        var uiLayer = GetCanvasLayer(LayerId.UI);
        // hearts, time
        var heartScene = ResourceLoader.Load(AssetPath.ENTITIES + "HeartHUD.tscn") as PackedScene;
        _heartHUD = heartScene.Instantiate() as HeartHUD;
        _heartHUD.Initialize();
        var timeScene = ResourceLoader.Load(AssetPath.ENTITIES + "TimeHUD.tscn") as PackedScene;
        _timeHUD = timeScene.Instantiate() as TimeHUD;
        uiLayer.AddChild(_heartHUD);
        uiLayer.AddChild(_timeHUD);
	}
    private void FruitCollectedHandler (int score) {
        _score += score;
        _heartHUD.SetScore(_score);
    }
    private Timer _timer;
    private int _elapsed_miliseconds = 0;
    private bool _is_start = false;
    private void GameStartedHandler () {
        _start_time = DateTime.Now;
        _is_start = true;
        _timer = new Timer
        {
            WaitTime = 0.04,
            OneShot = false
        };
        _timer.Timeout += TimerLoop;
        AddChild(_timer);
        _timer.Start();
    }
    private int ElapsedSeconds () {
        if (!_is_start)
            return 0;
        var currentTime = DateTime.Now;
        var timeSpan = currentTime - _start_time;
        return (int)timeSpan.TotalMilliseconds;
    }
    private void TimerLoop () {
        _elapsed_miliseconds = ElapsedSeconds();
        _timeHUD.SetTime(_elapsed_miliseconds);
    }
    private void GameWonHandler () {
        if (_is_start)
            _timer.Stop();
        AddPanelResult(true, _score, _elapsed_miliseconds);
    }
    private void GameLostHandler () {
        if (_is_start)
            _timer.Stop();
        AddPanelResult(false, _score, _elapsed_miliseconds);
    }
    private void HeartLostHandler (int currentHP){
        _heartHUD.SetHeart(currentHP);
    }
    private void AddEventHandlers () {
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.FruitCollected += FruitCollectedHandler;
        gameSystem.GameStarted += GameStartedHandler;
        gameSystem.GameWon += GameWonHandler;
        gameSystem.GameLost += GameLostHandler;
        gameSystem.HeartLost += HeartLostHandler;
    }
    public override void _ExitTree()
    {
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.FruitCollected -= FruitCollectedHandler;
        gameSystem.GameStarted -= GameStartedHandler;
        gameSystem.GameWon -= GameWonHandler;
        gameSystem.GameLost -= GameLostHandler;
        gameSystem.HeartLost -= HeartLostHandler;
    }

}
