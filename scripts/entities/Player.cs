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
	private Node2D _dust;
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
	private const double MAX_DIAGONAL_JUMP_VELOCITY = 500f;
	private const double MOVE_VELOCITY = 300f;
	public const double JUMP_VELOCITY = - 400f;
	private const double GRAVITY = 1200f;
	private const double JERK = 2000f;
	private const double INVULNERABILITY_PERIOD = 0.5;
	// move slower while in the air
	private double MOVE_VELOCITY_AIR_RATIO = 0.7;
	private const int TERRAIN_SET_BOUNCE = 2;
	private const double TERRAIN_BOUNCE_VELOCITY = -800;
	private int _jump_times = 0;
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
	}
    public override void _ExitTree()
    {
		var inputSystem = GetNode<InputSystem>(AutoLoad.INPUT_SYSTEM);
		inputSystem.ButtonPressed -= InputProcess;
    }
    private void Initialize () {
		AddToGroup(GroupName.PLAYER);
		_anim = GetNode<AnimatedSprite2D>("sprite");
		_dust = GetNode<Node2D>("Dust");
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
		if (_state is PlayerDieState) return;
		double velocity_x = _velocity_x;

		if (velocity_x > MOVE_VELOCITY)
			velocity_x = MOVE_VELOCITY;

		if (_direction == MoveDirection.LEFT)
			velocity_x = -velocity_x;
		// move slower while in the air
		if (_state is PlayerJumpState) 
			velocity_x *= MOVE_VELOCITY_AIR_RATIO;

		// update gravity
		_gravity += JERK * delta;
		// maximum gravity reached
		if (_gravity > GRAVITY)
			_gravity = GRAVITY;
		_velocity_y += _gravity * delta;
		Velocity = new Vector2((float)velocity_x, (float)_velocity_y);
		MoveAndSlide();
		if (IsOnFloor()) {
			_jump_times = 0;
			_velocity_y = 0;
			_is_jump_from_ground = true;
			HandleEvent(EventType.ON_FLOOR);
		}
		if (_velocity_y > 0 && !IsOnFloor()) {
			// fall
			HandleEvent(EventType.FALLING);
		}
		if (IsOnWall() && _state is PlayerJumpState) {
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
		_label2.Text = GetSlideCollisionCount().ToString();
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
	public void Jump (double initVel = JUMP_VELOCITY, bool indirectForce = false) {
		SetJumpVelocity(true, initVel);
		_anim.Play("Jump");
		
		// platform propels player
		if (indirectForce) return;

		// player presses jump button

		_jump_times += 1;
		var scene = ResourceLoader.Load(AssetPath.ENTITIES + "VFX.tscn") as PackedScene;
		var vfx = scene.Instantiate() as AnimatedSprite2D;
		var globalPoint = ToGlobal(_dust.Position);
		vfx.Position = globalPoint;
		GetParent().AddChild(vfx);
		vfx.Play("Jump");
		vfx.AnimationFinished += () => {
			if (vfx.Animation == "Jump")
			vfx.QueueFree();
		};
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
}
