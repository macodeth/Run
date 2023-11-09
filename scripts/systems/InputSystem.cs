using Godot;
using System;

public enum InputEventType {
	LEFT_JUST_PRESSED,
	RIGHT_JUST_PRESSED,
	LEFT_STILL_PRESSED,
	RIGHT_STILL_PRESSED,
	LEFT_RELEASED,
	RIGHT_RELEASED,
	UP,
	RESET_PRESSED
}


public partial class InputSystem : Node
{
	[Signal]
	public delegate void ButtonPressedEventHandler(InputEventType type);
    public override void _PhysicsProcess(double delta)
    {
		if (Input.IsActionPressed("Left")) {
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.LEFT_STILL_PRESSED);
		}
		if (Input.IsActionPressed("Right")) {
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.RIGHT_STILL_PRESSED);
		}
    }
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("Left")) {
			StaticUtil.Log("press left");
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.LEFT_JUST_PRESSED);
		}
		if (@event.IsActionPressed("Right")) {
			StaticUtil.Log("press right");
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.RIGHT_JUST_PRESSED);
		}
		if (@event.IsActionPressed("Up")) {
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.UP);
		}
		if (@event.IsActionReleased("Left")) {
			StaticUtil.Log("release left");
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.LEFT_RELEASED);
		}
		if (@event.IsActionReleased("Right")) {
			StaticUtil.Log("release right");
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.RIGHT_RELEASED);
		}
		if (@event.IsActionPressed("Reset")) {
			StaticUtil.Log("press reset");
			EmitSignal(SignalName.ButtonPressed, (int)InputEventType.RESET_PRESSED);
		}
    }
}
