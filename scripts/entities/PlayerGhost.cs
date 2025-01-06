using Godot;
using System;

public partial class PlayerGhost : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var tw = GetTree().CreateTween();
		tw.TweenProperty(this, "modulate:a", 1, 0.15);
		tw.TweenProperty(this, "modulate:a", 0, 0.15);
		tw.TweenCallback(Callable.From(() => {
			QueueFree();
		}));
	}
	public void SetDirection (bool isLeft) {
		FlipH = isLeft;
	}
}
