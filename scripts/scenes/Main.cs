using Enum;
using Godot;
using System;

public partial class Main : BaseScene
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		var BPlay = GetNode<Button>("BPlay");
		BPlay.Pressed += ClickStart;
		var BQuit = GetNode<Button>("BQuit");
		BQuit.Pressed += ClickQuit;
		var BContinue = GetNode<Button>("BContinue");
		BContinue.Pressed += ClickContinue;
	}

	private void ClickQuit () {
		GetTree().Quit();
	}
	private void ClickStart () {
		var util = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		util.LoadScene(SceneId.SELECTION);
	}
	private void ClickContinue () {
		int level = SaveSystem.GetLevel();
		var nodeUtil = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		if (level == 0)
			nodeUtil.LoadScene(SceneId.TUTORIAL);
		else
			nodeUtil.LoadLevel(level);
	}
}
