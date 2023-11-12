using Enum;
using Godot;
using System;

public partial class Start : Area2D
{
	private AnimatedSprite2D _anim;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("sprite");
		Idle();
		BodyEntered += BodyEnteredHandler;
	}
	private void BodyEnteredHandler (Node2D body) {
		if (body.IsInGroup(GroupName.PLAYER))
			Moving();
	}
	private void Idle () {
		_anim.Play("Idle");
	}
	private void Moving () {
		_anim.Play("Moving");
	}
}
