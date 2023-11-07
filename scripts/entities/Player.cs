using Enum;
using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private AnimatedSprite2D _anim;
	private PlayerState _state;
	private MoveDirection _direction;
	private float _velocityX = 0;
	private float _velocityY = 0;
	public MoveDirection Direction {
		get => _direction;
		set {
			_direction = value;
			var scale = Math.Abs(this._anim.Scale.X);
			if (_direction == MoveDirection.LEFT)
				this._anim.Scale = new Vector2(-scale, scale);
			else
				this._anim.Scale = new Vector2(scale, scale);
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	private void InputProcess (InputType type) {
		PlayerState state = _state.HandleInput(this, type);
		if (state != null) {
			StaticUtil.Log("state changed to " + state.GetType());
			_state = state;
			_state.Enter(this);
		}
	}
    public void Idle () {
		_anim.Play("Idle");
	}
	public void Run () {
		_anim.Play("Run");
	}
	public void Jump () {
		_anim.Play("Jump");
	}
}
