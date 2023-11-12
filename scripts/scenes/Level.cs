using Enum;
using Godot;
using System;

public partial class Level : BaseScene
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddBackground();
		AddUI();
	}

	private void AddBackground () {
        // add static background
        var canvasLayer = new CanvasLayer
        {
            Layer = (int)LayerId.BACKGROUND
        };
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
        canvasLayer.AddChild(bkg);

        AddChild(canvasLayer);
	}

	private void AddUI () {

	}

}
