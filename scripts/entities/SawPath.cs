using Godot;
using System;

enum LoopType {
	CIRCULAR,
	RETURN
}

public partial class SawPath : Path2D
{
	private PathFollow2D _pathFollow;
	[Export]
	private LoopType type;
	[Export]
	private double duration;
	public override void _Ready()
	{
		_pathFollow = GetNode<PathFollow2D>("PathFollow2D");
		if (type == LoopType.RETURN) {
			var tween = GetTree().CreateTween();
			tween.TweenProperty(_pathFollow, "progress_ratio", 1, duration);
			tween.TweenProperty(_pathFollow, "progress_ratio", 0, duration);
			tween.SetLoops();
		}
	}
    public override void _Process(double delta)
    {
		if (type == LoopType.CIRCULAR)
			_pathFollow.ProgressRatio += (float)delta;
    }

}
