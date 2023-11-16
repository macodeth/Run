using Enum;
using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private int _hp;
	public int HP {
		get => _hp;
	}
	private AnimatedSprite2D _anim;
	private Label _label;
	public Label _label2;
	private PlayerState _state;
	private PlayerState _prev_state;
	private MoveDirection _direction;
	private double _velocity_x = 0;
	private double _velocity_y = 0;
	public double VelocityY {
		get => _velocity_y;
	}
	private double _gravity = 0;
	private const double MOVE_VELOCITY = 300f;
	private const double JUMP_VELOCITY = - 350f;
	private const double GRAVITY = 1200f;
	private const double JERK = 2000f;
	private const double INVULNERABILITY_PERIOD = 0.5;
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
		Initialize();
		AddEventHandlers();
	}
    public override void _ExitTree()
    {
		var inputSystem = GetNode<InputSystem>(AutoLoad.INPUT_SYSTEM);
		inputSystem.ButtonPressed -= InputProcess;
    }
    private void Initialize () {
		AddToGroup(GroupName.PLAYER);
		_anim = GetNode<AnimatedSprite2D>("sprite");
		_label = GetNode<Label>("Label");
		_label2 = GetNode<Label>("Label2");
		ChangeState(new PlayerAppearState());
		_direction = MoveDirection.RIGHT;
		_gravity = GRAVITY;
		_hp = GameSystem.HP;
	}
	private void AddEventHandlers () {
		var inputSystem = GetNode<InputSystem>(AutoLoad.INPUT_SYSTEM);
		inputSystem.ButtonPressed += InputProcess;
		_anim.AnimationFinished += AnimationFinishedHandler;
	}
    public override void _PhysicsProcess(double delta)
    {
		double velocity = _velocity_x;
		if (_direction == MoveDirection.LEFT)
			velocity = -velocity;
		if (_state is PlayerJumpState) 
			velocity *= MOVE_VELOCITY_AIR_RATIO;
		// update gravity
		_gravity += JERK * delta;
		// maximum gravity reached
		if (_gravity > GRAVITY)
			_gravity = GRAVITY;
		_velocity_y += _gravity * delta;
		Velocity = new Vector2((float)velocity, (float)_velocity_y);
		MoveAndSlide();
		
		if (IsOnFloor()) {
			_velocity_y = 0;
			HandleEvent(EventType.ON_FLOOR);
		}
		if (_velocity_y > 0 && !IsOnFloor()) {
			// fall
			HandleEvent(EventType.FALLING);
		}
		for (int i = 0; i < GetSlideCollisionCount(); i++) {
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider() as Node;
			if (collider.IsInGroup(GroupName.TOP_BREAKABLE) && _prev_state is PlayerFallState) {
				var box = collider as Box;
				if (collision.GetNormal().Y < 0) {
					box.Hit();
				}
			}
			if (collider is Spike) {
				InstantDeath();
			}
		}
    }

    private void ChangeState (PlayerState state) {
		if (state != null) {
			_label.Text = state.GetType().ToString();
			StaticUtil.Log(state.ToString());
			_prev_state = _state;
			_state = state;
			_state.Enter(this);
		}
	}
	private void InputProcess (InputEventType type) {
		PlayerState state = _state.HandleInput(this, type);
		ChangeState(state);
	}
	public void SetJumpVelocity (bool isSet, double initVel) {
		_velocity_y = isSet ? initVel : 0;
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
	public void Jump (double initVel = JUMP_VELOCITY) {
		SetJumpVelocity(true, initVel);
		_anim.Play("Jump");
	}
	public void Die () {
		SetMoveVelocity(false);
		SetJumpVelocity(false, 0);
		_anim.Play("Die");
	}
	public void Appear () {
		_anim.Play("Appear");
	}
	public void Fall () {
		_anim.Play("Fall");
	}
	public void Hit () {
		_anim.Play("Hit");
	}
	private bool _took_damage = false;
	private DateTime _last_time_damaged;
	public void TakeDamage (int damage) {
		if (_state is PlayerHitState) return;
		DateTime current_time = DateTime.Now;
		if (_took_damage) {
			var timeSpan = current_time - _last_time_damaged;
			if ((int)timeSpan.TotalSeconds <= INVULNERABILITY_PERIOD) {
				return;
			}
		}
		_took_damage = true;
		_last_time_damaged = current_time;
		_hp -= damage;
		ChangeState(new PlayerHitState());
		var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
		gameSystem.EmitSignal(GameSystem.SignalName.HeartLost, _hp);
	}
	public void InstantDeath () {
		TakeDamage(_hp);
	}
	private void HandleEvent (EventType eventType) {
		PlayerState state = _state.HandleEvent(eventType);
		ChangeState(state);
	}
	private void AnimationFinishedHandler () {
		PlayerState state = _state.Exit(this);
		ChangeState(state);
	}
	public void EndGame () {
		var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
		gameSystem.EmitSignal(GameSystem.SignalName.GameLost);
		QueueFree();
	}
}
