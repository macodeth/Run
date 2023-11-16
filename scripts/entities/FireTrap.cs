using System;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Godot;

enum FireTrapState {
	ON,
	OFF,
	HIT
}

public partial class FireTrap : StaticBody2D
{
	[Export]
	public int Damage = 1;
	private AnimatedSprite2D _anim;
	private FireTrapState _state;
	private Timer _timer;
	private Area2D _trigger;
	private Area2D _fire;
	private CollisionShape2D _fireShape;
	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("Sprite");
		_trigger = GetNode<Area2D>("Trigger");
		_fire = GetNode<Area2D>("Fire");
		_fireShape = _fire.GetNode<CollisionShape2D>("Shape");
		Off();
		_trigger.BodyEntered += TriggerBodyEnteredHandler;
		_anim.AnimationFinished += AnimationFinishedHandler;
		_anim.FrameChanged += FrameChangedHandler;
		_fire.BodyEntered += FireBodyEnteredHandler;
		_fire.BodyExited += FireBodyExitedHandler;
	}
	private void FrameChangedHandler () {
		// iterate over contacted bodies
		// then apply damage
		if (_state == FireTrapState.ON) {
			foreach (var _body in _bodyList) {
				if (_body.IsInGroup(GroupName.PLAYER)) {
					var player = _body as Player;
					player.TakeDamage(Damage);
				}
			}
		}
	}
	private void On () {
		_fireShape.SetDeferred("disabled", false);
		_state = FireTrapState.ON;
		_anim.Play("On");
        _timer = new Timer
        {
            WaitTime = 2,
            OneShot = true
        };
        _timer.Timeout += () => {
			Off();
			_timer.Stop();
		};
		AddChild(_timer);
		_timer.Start();
	}
	private void Off () {
		_fireShape.SetDeferred("disabled", true);
		_state = FireTrapState.OFF;
		_anim.Play("Off");
	}
	private void Hit () {
		_fireShape.SetDeferred("disabled", true);
		_state = FireTrapState.HIT;
		_anim.Play("Hit");
	}
	private void AnimationFinishedHandler () {
		var animation = _anim.Animation;
		if (animation == "Hit") {
			On();
		}
	}
	private void TriggerBodyEnteredHandler (Node2D body) {
		if (body.IsInGroup(GroupName.PLAYER)) {
			if (_state == FireTrapState.OFF) {
				Hit();
			}
		}
	}
	private List<Node2D> _bodyList = new();
	private void FireBodyEnteredHandler (Node2D body) {
		if (_bodyList.IndexOf(body) == -1)
			_bodyList.Add(body);
	}
	private void FireBodyExitedHandler (Node2D body) {
		_bodyList.Remove(body);
	}
}
