using Godot;
using System;

public partial class FallingPlatform : MoveablePlatform
{
	protected AnimationPlayer _anim;
	protected Sprite2D _sprite;
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
					Push();
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
			QueueFree();
		}
    }
	private void Idle () {
		_anim.Play("Idle");
	}
	public void Push () {
		_anim.Play("Fall");
	}
	protected override void TopBodyEnteredHandler (Node2D body) {
		base.TopBodyEnteredHandler(body);
		CheckFall();
	}
}
