using Enum;
using Godot;
using System;

public partial class HeartHUD : Control
{
	[Export]
	public Label TScore;
	[Export]
	public BoxContainer HeartContainer;
	public void Initialize () {
		// initialize hearts
		var heart = GameSystem.HP;
		SetHeart(heart);
	}
	public void SetScore (int score) {
		TScore.Text = score.ToString();
	}
	public void SetHeart (int heart) {
		StaticUtil.RemoveAllChildren(HeartContainer);
		var scene = ResourceLoader.Load(AssetPath.ENTITIES + "Heart.tscn") as PackedScene;
		for (var i = 0; i < heart; i++) {
			var heartNode = scene.Instantiate();
			HeartContainer.AddChild(heartNode);
		}
	}
}
