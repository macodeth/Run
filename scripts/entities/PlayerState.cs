using Enum;
using Godot;
using System;

public abstract class PlayerState
{
	public abstract PlayerState HandleInput (Player player, InputType type);
	public abstract void Enter (Player player);
}

public class PlayerIdleState : PlayerState {
	public override void Enter (Player player) {
		player.Idle();
	}
	public override PlayerState HandleInput (Player player, InputType type) {
		switch (type) {
			case InputType.LEFT_PRESSED:
				return new PlayerRunState(MoveDirection.LEFT);
			case InputType.RIGHT_PRESSED:
				return new PlayerRunState(MoveDirection.RIGHT);
			case InputType.UP:
				return new PlayerJumpState();
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
	public override PlayerState HandleInput (Player player, InputType type) {
		switch (type) {
			case InputType.LEFT_PRESSED:
				return new PlayerRunState(MoveDirection.LEFT);
			case InputType.RIGHT_PRESSED:
				return new PlayerRunState(MoveDirection.RIGHT);
			case InputType.LEFT_RELEASED:
				if (player.Direction == MoveDirection.LEFT) {
					return new PlayerIdleState();
				} else {
					return null;
				}
			case InputType.RIGHT_RELEASED:
				if (player.Direction == MoveDirection.RIGHT) {
					return new PlayerIdleState();
				} else {
					return null;
				}
			case InputType.UP:
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
    public override PlayerState HandleInput (Player player, InputType type) {
		return new PlayerIdleState();
	}
}