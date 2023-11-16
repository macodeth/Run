using Enum;
using Godot;
using System;

public partial class Selection : BaseScene
{
	public readonly static int MAX_LEVEL = 10;
	public override void _Ready()
	{
		base._Ready();
		var buttonContainer = GetNode("MarginContainer/ButtonContainer");
		for (var i = 0; i < MAX_LEVEL; i++) {
			var scene = ResourceLoader.Load(AssetPath.ENTITIES + "LevelSelectButton.tscn") as PackedScene;
			var button = scene.Instantiate() as LevelSelectButton;
			button.SetLevel(i + 1);
			buttonContainer.AddChild(button);
		}
	}
}
