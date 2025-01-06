using Enum;
using Godot;
using System;
using System.Collections;

public abstract class PlayerState
{
	public virtual PlayerState HandleInput (Player player, InputEventType type) {
		return null;
	}
	public virtual PlayerState HandleEvent (EventType type) {
		return null;
	}
	public abstract void Enter (Player player);
	public virtual PlayerState Exit (Player player) {
		return null;
	}
}

public class PlayerIdleState : PlayerState {
	public override void Enter (Player player) {
		player.Idle();
	}
	public override PlayerState HandleInput (Player player, InputEventType type) {
		switch (type) {
			case InputEventType.LEFT_JUST_PRESSED:
			case InputEventType.LEFT_STILL_PRESSED:
				return new PlayerRunState(MoveDirection.LEFT);
			case InputEventType.RIGHT_JUST_PRESSED:
			case InputEventType.RIGHT_STILL_PRESSED:
				return new PlayerRunState(MoveDirection.RIGHT);
			case InputEventType.UP:
				if (player.IsOnFloor())
					return new PlayerJumpState(Player.JUMP_VELOCITY);
				return null;
			case InputEventType.RESET_PRESSED:
				return new PlayerDieState();
			case InputEventType.DASH_PRESSED:
				if (player.IsDashable())
					return new PlayerDashState();
				return null;
		}
		return null;
	}
    public override PlayerState HandleEvent(EventType type)
    {
		switch (type) {
			case EventType.FALLING:
				return new PlayerFallState();
		}
		return null;
    }
}

public class PlayerRunState : PlayerState {
    public override void Enter(Player player)
    {
		player.Run();
		player.VFXRun.Emitting = true;
		if (_direction == MoveDirection.RIGHT) {
			player.VFXRun.Gravity = new Vector2(-50, 0);
			player.VFXRun.Direction = new Vector2(-1, 0);
		} else {
			player.VFXRun.Gravity = new Vector2(50, 0);
			player.VFXRun.Direction = new Vector2(1, 0);
		}
		player.Direction = _direction;
    }
	private MoveDirection _direction;
    public PlayerRunState (MoveDirection direction) {
		_direction = direction;
	}
	public override PlayerState HandleInput (Player player, InputEventType type) {
		switch (type) {
			case InputEventType.LEFT_JUST_PRESSED:
				return new PlayerRunState(MoveDirection.LEFT);
			case InputEventType.RIGHT_JUST_PRESSED:
				return new PlayerRunState(MoveDirection.RIGHT);
			case InputEventType.LEFT_RELEASED:
				if (player.Direction == MoveDirection.LEFT) {
					return new PlayerIdleState();
				} else {
					return null;
				}
			case InputEventType.RIGHT_RELEASED:
				if (player.Direction == MoveDirection.RIGHT) {
					return new PlayerIdleState();
				} else {
					return null;
				}
			case InputEventType.UP:
				if (player.IsOnFloor())
					return new PlayerJumpState(Player.JUMP_VELOCITY);
				return null;
			case InputEventType.DASH_PRESSED:
				if (player.IsDashable())
					return new PlayerDashState();
				return null;
		}
		return null;
	}
	public override PlayerState HandleEvent(EventType type)
    {
		switch (type) {
			case EventType.FALLING:
				return new PlayerFallState();
		}
		return null;
    }
}

public class PlayerJumpState : PlayerState {
    public override void Enter(Player player)
    {
		player.VFXJump.Restart();
		player.VFXJump.Emitting = true;
		player.Jump(_initVel, _indirectForce);
		player.PlayAudio("Jump");
    }
	private double _initVel = 0;
	private bool _indirectForce = false;
	public PlayerJumpState (double initVel, bool indirectForce = false) {
		_initVel = initVel;
		_indirectForce = indirectForce;
	}
    public override PlayerState HandleInput (Player player, InputEventType type) {
		switch (type) {
			case InputEventType.LEFT_JUST_PRESSED:
			case InputEventType.LEFT_STILL_PRESSED:
				player.Direction = MoveDirection.LEFT;
				player.SetMoveVelocity(true);
				return null;
			case InputEventType.RIGHT_JUST_PRESSED:
			case InputEventType.RIGHT_STILL_PRESSED:
				player.Direction = MoveDirection.RIGHT;
				player.SetMoveVelocity(true);
				return null;
			case InputEventType.LEFT_RELEASED:
				if (player.Direction == MoveDirection.LEFT) {
					player.SetMoveVelocity(false);
				}
				return null;
			case InputEventType.RIGHT_RELEASED:
				if (player.Direction == MoveDirection.RIGHT) {
					player.SetMoveVelocity(false);
				}
				return null;
			case InputEventType.UP:
				if (player.IsJumpable())
					return new PlayerJumpState(Player.JUMP_VELOCITY);
				return null;
			case InputEventType.DASH_PRESSED:
				if (player.IsDashable())
					return new PlayerDashState();
				return null;
		}
		return null;
	}
	public override PlayerState HandleEvent(EventType type)
    {
		switch (type) {
			case EventType.FALLING:
				return new PlayerFallState();
		}
		return null;
    }
}

public class PlayerDieState : PlayerState {
    public override void Enter(Player player)
    {
		player.Die();
		player.PlayAudio("Die");
    }
    public override PlayerState Exit(Player player)
    {
		player.EndGame();
		return null;
    }
}

public class PlayerFallState : PlayerState {
	public override void Enter(Player player) 
	{
		player.SetFirstFallTime();
		player.Fall();
	}
	public override PlayerState Exit(Player player) 
	{
		return null;
	}
	public override PlayerState HandleEvent(EventType type)
    {
		switch (type) {
			case EventType.ON_FLOOR:
				return new PlayerIdleState();
		}
		return null;
    }
	public override PlayerState HandleInput (Player player, InputEventType type) {
		switch (type) {
			case InputEventType.LEFT_JUST_PRESSED:
			case InputEventType.LEFT_STILL_PRESSED:
				player.Direction = MoveDirection.LEFT;
				player.SetMoveVelocity(true);
				return null;
			case InputEventType.RIGHT_JUST_PRESSED:
			case InputEventType.RIGHT_STILL_PRESSED:
				player.Direction = MoveDirection.RIGHT;
				player.SetMoveVelocity(true);
				return null;
			case InputEventType.LEFT_RELEASED:
				if (player.Direction == MoveDirection.LEFT) {
					player.SetMoveVelocity(false);
				}
				return null;
			case InputEventType.RIGHT_RELEASED:
				if (player.Direction == MoveDirection.RIGHT) {
					player.SetMoveVelocity(false);
				}
				return null;
			case InputEventType.UP:
				if (player.IsJumpable()) {
					if (!player.IsCoyoteTime())
						player.SetMaxMinus1Jump();
					return new PlayerJumpState(Player.JUMP_VELOCITY);
				}
				else
					player.SetJumpBuffer();
				return null;
			case InputEventType.DASH_PRESSED:
				if (player.IsDashable())
					return new PlayerDashState();
				return null;
		}
		return null;
	}
}

public class PlayerHitState : PlayerState {
    public override void Enter(Player player)
    {
		player.Hit();
		player.PlayAudio("Hurt");
    }
    public override PlayerState Exit(Player player)
    {
		if (player.HP <= 0) {
			return new PlayerDieState();
		}
		if (player.VelocityY < 0)
			return new PlayerFallState();
		return new PlayerIdleState();
    }
}

public class PlayerDashState : PlayerState {
    public override void Enter(Player player)
    {
		player.Dash();
		player.PlayAudio("Dash");
    }
    public override PlayerState HandleInput(Player player, InputEventType type)
    {	
		switch (type) 
		{
			case InputEventType.UP:
				if (player.IsJumpable())
					return new PlayerJumpState(Player.JUMP_VELOCITY);
				else
					player.SetJumpBuffer();
				break;
		}
		return null;
    }
}