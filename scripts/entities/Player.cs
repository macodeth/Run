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
	private double _y_before_jump = 0;
	private readonly double MOVE_VELOCITY = 300f;
	private readonly double JUMP_VELOCITY = 350f;
	private readonly double GRAVITY = 600.0f;
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
		_state = new PlayerIdleState();
		_direction = MoveDirection.RIGHT;
		Idle();
		var inputSystem = GetNode<InputSystem>("/root/InputSystem/");
		inputSystem.ButtonPressed += InputProcess;
		JumpFinished += JumpFinishedHandler;
	}
    public override void _Process(double delta)
    {
		double velocity = _velocity_x;
		if (_direction == MoveDirection.LEFT)
			velocity = -velocity;
		double newX = velocity * delta + Position.X;
		if (_state is PlayerIdleState || _state is PlayerRunState) {
			Position = new Vector2((float)newX, Position.Y);
			return;
		}
		_velocity_y -= GRAVITY * delta;
		double newY = 0;
		if ((_velocity_y < -JUMP_VELOCITY)
			&& (_state is PlayerJumpState)) {
			_velocity_y = 0;
			newY = _y_before_jump;
			EmitSignal(SignalName.JumpFinished);
		} else {
			newY = Position.Y - _velocity_y * delta + 0.5 * GRAVITY * delta * delta;
		}
		Position = new Vector2((float)newX, (float)newY);
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
		_y_before_jump = Position.Y;
	}
	[Signal]
	public delegate void JumpFinishedEventHandler();
	private void JumpFinishedHandler () {
		StaticUtil.Log("jump finished");
		PlayerState state = _state.HandleEvent(EventType.JUMP_FINISHED);
		ChangeState(state);
	}
}
