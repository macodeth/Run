using Godot;
using System;

public partial class FallingPlatform : MoveablePlatform
{
	private double MOVE_DISTANCE = 200;
	private double APPEAR_DISTANCE = 50;
	private double APPEAR_TIME = 0.2;
	private double MOVE_TIME = 0.6;
	protected AnimationPlayer _anim;
	protected Sprite2D _sprite;
	private Vector2 _origin;
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
		_origin = Position;
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
			var timer = new Timer {
				WaitTime = 5,
				OneShot = true
			};
			timer.Timeout += () => {
				var tween = CreateTween();
				tween.SetParallel();
				Position = new Vector2(_origin.X, _origin.Y + (float)APPEAR_DISTANCE);
				tween.TweenProperty(this, "position:y", _origin.Y, APPEAR_TIME);
				tween.TweenProperty(this, "modulate:a", 255, APPEAR_TIME);
				tween.TweenCallback(Callable.From(() => {
					EnableCollision();
					Idle();
				}));
			};
			AddChild(timer);
			timer.Start();
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
