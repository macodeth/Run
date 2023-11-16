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
		var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
		var nodeUtil = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		nodeUtil.LoadLevel(gameSystem.Level);
	}
	private void ClickNext () {
		var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
		var nodeUtil = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		nodeUtil.LoadLevel(gameSystem.Level + 1);
	}
	public void SetData (bool isWin, int score, int seconds) {
		TTime.Text = StaticUtil.TimeFormat(seconds);
		if (isWin)
			TResult.Text = "STAGE COMPLETED!";
		else
			TResult.Text = "DEATH ...";
		TScore.Text = score.ToString();
		if (!isWin)
			BNext.Hide();
		var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
		if (gameSystem.Level > Selection.MAX_LEVEL)
			BNext.Hide();
	}
}
