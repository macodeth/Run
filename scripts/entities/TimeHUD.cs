using Godot;
using System;

public partial class TimeHUD : Control
{
	[Export]
	public Label _label;
	public void SetTime (int seconds) {
		_label.Text = StaticUtil.TimeFormat(seconds);
	}
}
