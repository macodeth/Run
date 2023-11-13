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

    }
    private Timer _timer;
    private void GameStartedHandler () {
        _start_time = DateTime.Now;
        _timer = new Timer
        {
            WaitTime = 0.5,
            OneShot = false
        };
        _timer.Timeout += TimerLoop;
        AddChild(_timer);
        _timer.Start();
    }
    private int ElapsedSeconds () {
        var currentTime = DateTime.Now;
        var timeSpan = currentTime - _start_time;
        return (int)timeSpan.TotalSeconds;
    }
    private void TimerLoop () {
        var elapsedSeconds = ElapsedSeconds();
        _timeHUD.SetTime(elapsedSeconds);
    }
    private void GameEndedHandler () {
        _timer.Stop();
    }
    private void AddEventHandlers () {
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.FruitCollected += FruitCollectedHandler;
        gameSystem.GameStarted += GameStartedHandler;
        gameSystem.GameEnded += GameEndedHandler;
    }

}
