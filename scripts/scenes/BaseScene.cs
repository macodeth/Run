using System.Collections.Generic;
using Enum;
using Godot;

public partial class BaseScene: Control {
    public override void _Ready()
    {
        AddLayers();
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