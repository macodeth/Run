using Enum;
using Godot;
using System;

public partial class End : Area2D
{
	private AnimatedSprite2D _anim;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("sprite");
		Idle();
		BodyEntered += BodyEnteredHandler;
	}
	private bool _is_end = false;
	private void BodyEnteredHandler (Node2D body) {
		if (body.IsInGroup(GroupName.PLAYER)) {
			if (!_is_end) {
				var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
				gameSystem.EmitSignal(GameSystem.SignalName.GameWon);
			}
			_is_end = true;
		}
	}
	private void Idle () {
		_anim.Play("Idle");
	}
}
