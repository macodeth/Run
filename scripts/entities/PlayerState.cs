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
		return this;
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
		return this;
    }
}

public class PlayerRunState : PlayerState {
    public override void Enter(Player player)
    {
		player.Run();
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
		return this;
    }
}

public class PlayerJumpState : PlayerState {
    public override void Enter(Player player)
    {
		player.Jump(_initVel, _indirectForce);
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
		return this;
    }
}

public class PlayerDieState : PlayerState {
    public override void Enter(Player player)
    {
		player.Die();
    }
    public override PlayerState Exit(Player player)
    {
		player.EndGame();
		return null;
    }
}

public class PlayerAppearState : PlayerState {
    public override void Enter(Player player)
    {
		player.Appear();
    }
    public override PlayerState Exit(Player player)
    {
		return new PlayerIdleState();
    }
	public override PlayerState HandleEvent(EventType type)
    {
		switch (type) {
			case EventType.FALLING:
				return new PlayerFallState();
		}
		return this;
    }
}

public class PlayerFallState : PlayerState {
	public override void Enter(Player player) 
	{
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
		return this;
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
}

public class PlayerHitState : PlayerState {
    public override void Enter(Player player)
    {
		player.Hit();
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
    }
    public override PlayerState HandleInput(Player player, InputEventType type)
    {
		return null;
    }
}