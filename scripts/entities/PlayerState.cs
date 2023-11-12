using Enum;
using Godot;
using System;

public abstract class PlayerState
{
	public virtual PlayerState HandleInput (Player player, InputEventType type) {
		return null;
	}
	public virtual PlayerState HandleEvent (EventType type) {
		return this;
	}
	public abstract void Enter (Player player);
	public virtual PlayerState Exit () {
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
				return new PlayerJumpState();
			case InputEventType.RESET_PRESSED:
				return new PlayerDieState();
		}
		return null;
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
				return new PlayerJumpState();
		}
		return null;
	}
}

public class PlayerJumpState : PlayerState {
    public override void Enter(Player player)
    {
		player.Jump();
    }
    public override PlayerState HandleEvent(EventType type)
    {
		switch (type) {
			case EventType.JUMP_FINISHED:
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
		}
		return null;
	}
}

public class PlayerDieState : PlayerState {
    public override void Enter(Player player)
    {
		player.Die();
    }
}

public class PlayerAppearState : PlayerState {
    public override void Enter(Player player)
    {
		player.Appear();
    }
    public override PlayerState Exit()
    {
		return new PlayerIdleState();
    }
}