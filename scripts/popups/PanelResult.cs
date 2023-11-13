using Enum;
using Godot;
using System;

public partial class PanelResult : BasePopup
{
	[Export]
	public Button BLevel;
	[Export]
	public Button BReplay;
	[Export]
	public Button BNext;
	[Export]
	public Label TResult;
	[Export]
	public Label TTime;
	[Export]
	public Label TScore;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BLevel.Pressed += ClickLevel;
		BReplay.Pressed += ClickReplay;
		BNext.Pressed += ClickNext;
	}

	private void ClickLevel () {
		var nodeUtil = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		nodeUtil.LoadScene(SceneId.SELECTION);
	}
	private void ClickReplay () {

	}
	private void ClickNext () {

	}
}
