using Enum;
using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Player : CharacterBody2D
{
	private AnimatedSprite2D _anim;
	private PlayerState _state;
	private PlayerState _prev_state;
	private MoveDirection _direction;
	private double _velocity_x = 0;
	private double _velocity_y = 0;
	private double _gravity = 0;
	private readonly double MOVE_VELOCITY = 300f;
	private readonly double JUMP_VELOCITY = - 350f;
	private readonly double GRAVITY = 1200f;
	private readonly double JERK = 600f;
	// move slower while in the air
	private readonly double MOVE_VELOCITY_AIR_RATIO = 0.7;
	public MoveDirection Direction {
		get => _direction;
		set {
			_direction = value;
			var scale = Math.Abs(_anim.Scale.X);
			if (_direction == MoveDirection.LEFT)
				_anim.Scale = new Vector2(-scale, scale);
			else
				_anim.Scale = new Vector2(scale, scale);
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup(GroupName.PLAYER);
		_anim = GetNode<AnimatedSprite2D>("sprite");
		ChangeState(new PlayerAppearState());
		_direction = MoveDirection.RIGHT;
		_gravity = GRAVITY;
		var inputSystem = GetNode<InputSystem>(AutoLoad.INPUT_SYSTEM);
		inputSystem.ButtonPressed += InputProcess;
		JumpFinished += JumpFinishedHandler;
		_anim.AnimationFinished += AnimationFinishedHandler;
	}
    public override void _PhysicsProcess(double delta)
    {
		double velocity = _velocity_x;
		if (_direction == MoveDirection.LEFT)
			velocity = -velocity;
		if (_state is PlayerJumpState) 
			velocity *= MOVE_VELOCITY_AIR_RATIO;
		_gravity += JERK * delta;
		if (_gravity > GRAVITY)
			_gravity = GRAVITY;
		_velocity_y += _gravity * delta;
		Velocity = new Vector2((float)velocity, (float)_velocity_y);
		MoveAndSlide();
		if (IsOnFloor()) {
			_velocity_y = 0;
			EmitSignal(SignalName.JumpFinished);
		}
		for (int i = 0; i < GetSlideCollisionCount(); i++) {
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider() as Node;
			if (collider.IsInGroup(GroupName.TOP_BREAKABLE) && _prev_state is PlayerJumpState) {
				var box = collider as Box;
				if (collision.GetNormal().Y < 0) {
					box.Hit();
				}
			}
			if (collider.IsInGroup(GroupName.FRUIT)) {
				var fruit = collider as Fruit;
				fruit.QueueFree();
			}
		}
    }

    private void ChangeState (PlayerState state) {
		if (state != null) {
			_prev_state = _state;
			_state = state;
			_state.Enter(this);
		}
	}
	private void InputProcess (InputEventType type) {
		PlayerState state = _state.HandleInput(this, type);
		ChangeState(state);
	}
	public void SetJumpVelocity (bool isSet) {
		_velocity_y = isSet ? JUMP_VELOCITY : 0;
		_gravity = 500;
	}
	public void SetMoveVelocity (bool isSet) {
		_velocity_x = isSet ? MOVE_VELOCITY : 0;
	}
    public void Idle () {
		SetMoveVelocity(false);
		_anim.Play("Idle");
	}
	public void Run () {
		SetMoveVelocity(true);
		_anim.Play("Run");
	}
	public void Jump () {
		SetJumpVelocity(true);
		_anim.Play("Jump");
	}
	public void Die () {
		SetMoveVelocity(false);
		SetJumpVelocity(false);
		_anim.Play("Die");
	}
	public void Appear () {
		_anim.Play("Appear");
	}
	[Signal]
	public delegate void JumpFinishedEventHandler();
	private void JumpFinishedHandler () {
		PlayerState state = _state.HandleEvent(EventType.JUMP_FINISHED);
		ChangeState(state);
	}
	private void AnimationFinishedHandler () {
		PlayerState state = _state.Exit();
		ChangeState(state);
	}
}
