using System;
using System.Collections.Generic;
using Enum;
using Godot;

public partial class BaseScene: Control {
    private Node2D audioSys;
    public override void _Ready()
    {
        AddLayers();
        AddAudioListener();
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.PlayAudio += PlayAudioHandler;
        gameSystem.PlayAudioLoop += PlayAudioLoopHandler;
        gameSystem.SetReverb += SetReverbHandler;
        gameSystem.SetDistance += SetDistanceHandler;
        gameSystem.StopAudio += StopAudioHandler;
    }
    private void StopAudioHandler () {
        audioSys.Call("stop_all");
    }
    protected void PlayAudioHandler (string name) {
        audioSys.Call("play_event", name);
    }
    private void PlayAudioLoopHandler (string name, ulong sawId) {
        audioSys.Call("play_event_loop", sawId, name);
    }
    private void SetReverbHandler (float val) {
        audioSys.Call("set_reverb", val);
    }
    private void SetDistanceHandler (ulong sawId, float val) {
        audioSys.Call("set_distance", sawId, val);
    }
    public override void _ExitTree()
    {
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.PlayAudio -= PlayAudioHandler;
        gameSystem.PlayAudioLoop -= PlayAudioLoopHandler;
        gameSystem.SetReverb -= SetReverbHandler;
        gameSystem.SetDistance -= SetDistanceHandler;
        gameSystem.StopAudio -= StopAudioHandler;
    }
    private void AddAudioListener () {
        var scene = ResourceLoader.Load(AssetPath.SCENES + "AudioSystem.tscn") as PackedScene;
        audioSys = scene.Instantiate() as Node2D;
        AddChild(audioSys);
    }
    private Dictionary<LayerId, CanvasLayer> CanvasList = new Dictionary<LayerId, CanvasLayer>();
    private void AddLayers () {
        var backgroundLayer = new CanvasLayer
        {
            Name = "CanvasLayer" + LayerId.BACKGROUND,
            Layer = (int)LayerId.BACKGROUND
        };
        var uiLayer = new CanvasLayer
        {
            Layer = (int)LayerId.UI
        };
        AddChild(uiLayer);
        AddChild(backgroundLayer);
        CanvasList.Add(LayerId.UI, uiLayer);
        CanvasList.Add(LayerId.BACKGROUND, backgroundLayer);
    }
    public CanvasLayer GetCanvasLayer (LayerId layerId) {
        return CanvasList[layerId];
    }
    public void AddPanelResult (bool isWin, int score, int seconds) {
        var popup = AddPopup(PopupType.RESULT) as PanelResult;
        popup.SetData(isWin, score, seconds);
    }
    private BasePopup AddPopup (PopupType type) {
        var name = "";
        switch (type) {
            case PopupType.RESULT:
                name = "PanelResult";
                break;
        }
        var uiLayer = GetCanvasLayer(LayerId.UI);
        var scene = ResourceLoader.Load(AssetPath.POPUP + name + ".tscn") as PackedScene;
        var popup = scene.Instantiate() as BasePopup;
        uiLayer.AddChild(popup);
        return popup;
    }
}