using Godot;
using System;

public partial class TimeHUD : Control
{
	[Export]
	public Label _label;
	public void SetTime (int seconds) {
		int minutes = seconds / 60;
		seconds %= 60;
		_label.Text = minutes.ToString().PadZeros(2) + ":" + seconds.ToString().PadZeros(2);
	}
}
