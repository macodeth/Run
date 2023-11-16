using Godot;
using System;

public partial class FallingPlatform : MoveablePlatform
{
	private double MOVE_DISTANCE = 200;
	private double MOVE_TIME = 0.6;
	protected AnimationPlayer _anim;
	protected Sprite2D _sprite;
    public override void _PhysicsProcess(double delta)
    {
		ObjectMove();
    }
    public override void _Ready()
	{
		base._Ready();
		_sprite = GetNode<Sprite2D>("Sprite");
		_anim = GetNode<AnimationPlayer>("Anim");
		_anim.AnimationFinished += AnimationFinishedHandler;
		_sprite.FrameChanged += FrameChangedHandler;
		Idle();
	}
	private void CheckFall () {
		foreach (var body in _bodyList) {
			if (body is Player) {
				if (_anim.CurrentAnimation == "Idle") {
					Fall();
				}
			}
		}
	}
	private void FrameChangedHandler () {
		CheckFall();
	}
    private void AnimationFinishedHandler(StringName animName)
    {
		if (animName == "Fall")
		{
			DisableCollision();
			var tween = CreateTween();
			tween.SetParallel();
			var y = Position.Y;
			tween.TweenProperty(this, "position:y", y + MOVE_DISTANCE, MOVE_TIME);
			tween.TweenProperty(this, "modulate:a", 0, MOVE_TIME);
		}
    }
	private void Idle () {
		ObjectStay();
		_anim.Play("Idle");
	}
	public void Fall () {
		_anim.Play("Fall");
	}
	protected override void TopBodyEnteredHandler (Node2D body) {
		base.TopBodyEnteredHandler(body);
		CheckFall();
	}
}
