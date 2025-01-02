using Godot;
using System;

public partial class TimeHUD : Control
{
	[Export]
	public Label _label;
	public void SetTime (int miliseconds) {
		_label.Text = "";
	}
}
