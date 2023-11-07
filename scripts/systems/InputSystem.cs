using Godot;
using System;

public enum InputType {
	LEFT_PRESSED,
	RIGHT_PRESSED,
	LEFT_RELEASED,
	RIGHT_RELEASED,
	UP
}

public partial class InputSystem : Node
{
	[Signal]
	public delegate void ButtonPressedEventHandler(InputType type);
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("Left")) {
			StaticUtil.Log("press left");
			EmitSignal(SignalName.ButtonPressed, (int)InputType.LEFT_PRESSED);
		}
		if (@event.IsActionPressed("Right")) {
			StaticUtil.Log("press right");
			EmitSignal(SignalName.ButtonPressed, (int)InputType.RIGHT_PRESSED);
		}
		if (@event.IsActionPressed("Up")) {
			EmitSignal(SignalName.ButtonPressed, (int)InputType.UP);
		}
		if (@event.IsActionReleased("Left")) {
			StaticUtil.Log("release left");
			EmitSignal(SignalName.ButtonPressed, (int)InputType.LEFT_RELEASED);
		}
		if (@event.IsActionReleased("Right")) {
			StaticUtil.Log("release right");
			EmitSignal(SignalName.ButtonPressed, (int)InputType.RIGHT_RELEASED);
		}
    }
}
