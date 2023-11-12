using Enum;
using Godot;
using System;

public partial class LevelSelectButton : Button
{
	private int level;
    public override void _Ready()
    {
		Pressed += ClickButton;
    }
    public void SetLevel (int level) {
        this.level = level;
		var label = GetNode("Label") as Label;
		label.Text = level.ToString();
	}
	public void ClickButton () {
		var util = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		util.LoadLevel(level);
	}
}
