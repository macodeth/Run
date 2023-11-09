using Enum;
using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private AnimatedSprite2D _anim;
	private PlayerState _state;
	private MoveDirection _direction;
	private double _velocity_x = 0;
	private double _velocity_y = 0;
	private double _gravity = 0;
	private readonly double MOVE_VELOCITY = 300f;
	private readonly double JUMP_VELOCITY = - 350f;
	private readonly double GRAVITY = 800.0f;
	private readonly double JERK = 400.0f;
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
		_anim = GetNode<AnimatedSprite2D>("sprite");
		ChangeState(new PlayerAppearState());
		_direction = MoveDirection.RIGHT;
		_gravity = GRAVITY;
		var inputSystem = GetNode<InputSystem>("/root/InputSystem/");
		inputSystem.ButtonPressed += InputProcess;
		JumpFinished += JumpFinishedHandler;
		_anim.AnimationFinished += AnimationFinishedHandler;
	}
    public override void _PhysicsProcess(double delta)
    {
		double velocity = _velocity_x;
		if (_direction == MoveDirection.LEFT)
			velocity = -velocity;
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
    }

    private void ChangeState (PlayerState state) {
		if (state != null) {
			StaticUtil.Log("state changed to " + state.GetType());
			_state = state;
			_state.Enter(this);
		}
	}
	private void InputProcess (InputEventType type) {
		PlayerState state = _state.HandleInput(this, type);
		ChangeState(state);
	}
	public void setVVelocity (bool isSet) {
		_velocity_y = isSet ? JUMP_VELOCITY : 0;
		_gravity = 500;
	}
	public void SetHVelocity (bool isSet) {
		_velocity_x = isSet ? MOVE_VELOCITY : 0;
	}
    public void Idle () {
		SetHVelocity(false);
		_anim.Play("Idle");
	}
	public void Run () {
		SetHVelocity(true);
		_anim.Play("Run");
	}
	public void Jump () {
		setVVelocity(true);
		_anim.Play("Jump");
	}
	public void Die () {
		SetHVelocity(false);
		setVVelocity(false);
		_anim.Play("Die");
	}
	public void Appear () {
		_anim.Play("Appear");
	}
	[Signal]
	public delegate void JumpFinishedEventHandler();
	private void JumpFinishedHandler () {
		StaticUtil.Log("jump finished");
		PlayerState state = _state.HandleEvent(EventType.JUMP_FINISHED);
		ChangeState(state);
	}
	private void AnimationFinishedHandler () {
		PlayerState state = _state.Exit();
		ChangeState(state);
	}
}
