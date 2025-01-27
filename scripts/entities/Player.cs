using Enum;
using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private int _hp;
	public int HP {
		get => _hp;
	}
	[Export]
	private PackedScene playerGhost;
	[Export]
	public CpuParticles2D VFXRun;
	[Export]
	public GpuParticles2D VFXJump;
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
	private const int MAX_JUMP = 2;
	private const double MOVE_VELOCITY = 300f;
	private const double DASH_VELOCITY = 800f;
	private const double MAX_DASH_TIME = 0.25f;
	private double _dash_time = 0;
	public const double JUMP_VELOCITY = - 400f;
	private const double GRAVITY = 1200f;
	private const double JERK = 2000f;
	private const double INVULNERABILITY_PERIOD = 0.5;
	// move slower while in the air
	private const double MOVE_VELOCITY_AIR_RATIO = 0.7;
	private const int TERRAIN_SET_BOUNCE = 2;
	private int _jump_times = 0;
	private bool _dashable = true;
	private const double JUMP_BUFFER_TIME = 0.25;
	private const double COYOTE_TIME = 0.25;
	private DateTime _jump_buffer = new();
	private DateTime _first_fall_time = new();
	private const float DASH_GHOST_DISTANCE = 60;
	private float _dash_ghost_pos = 0;
	public void SetFirstFallTime () {
		if (_prev_state is not PlayerFallState)
			_first_fall_time = DateTime.Now;
	}
	public bool IsDashable () {
		return _dashable;
	}
	public bool IsJumpable () {
		return _jump_times < MAX_JUMP;
	}
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
	// only increase one additional jump after touching wall
	private bool _is_on_wall_first = false;
	// prevent players from being able to jump additional time after touching wall
	private bool _is_jump_from_ground = true;
	public override void _Ready()
	{
		Initialize();
		AddEventHandlers();
		var audioListener = new AudioListener2D();
        AddChild(audioListener);
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
		ChangeState(new PlayerIdleState());
		_direction = MoveDirection.RIGHT;
		_gravity = GRAVITY;
		_hp = GameSystem.hp;
	}
	private void AddEventHandlers () {
		var inputSystem = GetNode<InputSystem>(AutoLoad.INPUT_SYSTEM);
		inputSystem.ButtonPressed += InputProcess;
		_anim.AnimationFinished += AnimationFinishedHandler;
	}
    public override void _PhysicsProcess(double delta)
    {
		if (_state is PlayerDieState) return;
		double velocity_x = _velocity_x;

		if (_direction == MoveDirection.LEFT)
			velocity_x = -velocity_x;
		// move slower while in the air
		if (_state is PlayerJumpState && _state is not PlayerDashState) 
			velocity_x *= MOVE_VELOCITY_AIR_RATIO;

		// update gravity
		_gravity += JERK * delta;
		// maximum gravity reached
		if (_gravity > GRAVITY)
			_gravity = GRAVITY;
		_velocity_y += _gravity * delta;
		Velocity = new Vector2(
			(_state is PlayerHitState)?0 : (float)velocity_x, 
			(_state is PlayerDashState || _state is PlayerHitState || _state is PlayerDieState) ? 0 : (float)_velocity_y);
		MoveAndSlide();
		// dash
		if (_state is PlayerDashState) {
			_dash_time += delta;
			float currentPos = Position.X;
			double distance = Math.Abs(currentPos - _dash_ghost_pos);
			if (distance > DASH_GHOST_DISTANCE) {
				StaticUtil.Log("spawn dash ghost");
				_dash_ghost_pos = currentPos;
				var dashGhost = playerGhost.Instantiate() as PlayerGhost;
				dashGhost.SetDirection(_direction == MoveDirection.LEFT);
				GetParent().AddChild(dashGhost);
				dashGhost.Position = Position;
			}
			if (_dash_time > MAX_DASH_TIME) {
				_dash_time = 0;
				_velocity_x = 0;
				if (IsOnFloor()) {
					if (IsJumpBufferValid())
						ChangeState(new PlayerJumpState(JUMP_VELOCITY));
					else
						ChangeState(new PlayerIdleState());
				}
				else
					ChangeState(new PlayerFallState());
			}
		}
		if (IsOnFloor()) {
			_jump_times = 0;
			_velocity_y = 0;
			_is_jump_from_ground = true;
			_dashable = true;
			if (IsJumpBufferValid())
				ChangeState(new PlayerJumpState(JUMP_VELOCITY));
			else
				HandleEvent(EventType.ON_FLOOR);
		}
		if (_velocity_y > 0 && !IsOnFloor()) {
			// fall
			HandleEvent(EventType.FALLING);
		}
		// wall jump
		if (IsOnWall() && (_state is PlayerJumpState || _state is PlayerFallState)) {
			if (!_is_on_wall_first) {
				if (!_is_jump_from_ground)
					_jump_times -= 1;
				_is_jump_from_ground = false;
				_is_on_wall_first = true;
				_jump_times = Math.Max(0, _jump_times);
			}
		}
		if (!IsOnWall()) {
			_is_on_wall_first = false;
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
			// players hit something above 
			// --> immediately stop
			if (collision.GetNormal().Y > 0) {
				_velocity_y = 0;
			}
			if (collider is TileMap) {
				var tilemap = collider as TileMap;
				var cell = tilemap.LocalToMap(collision.GetPosition() - collision.GetNormal() - new Vector2(1, 0));
				var tile = tilemap.GetCellTileData(0, cell);
				if (tile != null) {
					var set = tile.TerrainSet;
					if (set == TERRAIN_SET_BOUNCE && collision.GetNormal().Y < 0) {
						ChangeState(new PlayerJumpState(-900, true));
					}
				}
			}
		}
    }
    public void ChangeState (PlayerState state) {
		if (state != null) {
			// _label.Text = state.GetType().ToString();
			_prev_state = _state;
			_state = state;
			VFXRun.Emitting = false;
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
	public void SetDashVelocity (bool isSet) {
		_velocity_x = isSet ? DASH_VELOCITY : 0;
		_velocity_y = 0;
	}
    public void Idle () {
		SetMoveVelocity(false);
		_anim.Play("Idle");
	}
	public void Run () {
		SetMoveVelocity(true);
		_anim.Play("Run");
	}
	public void Jump (double initVel = JUMP_VELOCITY, bool indirectForce = false) {
		SetJumpVelocity(true, initVel);
		_anim.Play("Jump");
		
		// platform propels player
		if (indirectForce) return;

		// player presses jump button
		_jump_times += 1;
	}
	public void Dash () {
		SetDashVelocity(true);
		_dash_ghost_pos = Position.X;
		_anim.Play("Dash");
		_dashable = false;
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
		_velocity_y = 0;
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
	public void SetJumpBuffer () {
		_jump_buffer = DateTime.Now;
	}
	private bool IsJumpBufferValid () {
		if (_state is PlayerHitState || _state is PlayerDieState) return false;
		var elapsedTime = DateTime.Now - _jump_buffer;
		return elapsedTime.TotalSeconds < JUMP_BUFFER_TIME;
	}
	public bool IsCoyoteTime () {
		var elapsedTime = DateTime.Now - _first_fall_time;
		StaticUtil.Log(elapsedTime.ToString());
		return elapsedTime.TotalSeconds < COYOTE_TIME;
	}
	public void SetMaxMinus1Jump () {
		_jump_times = MAX_JUMP - 1;
	}
    public void PlayAudio (string name) {
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.EmitSignal(GameSystem.SignalName.PlayAudio, name);
    }
}
