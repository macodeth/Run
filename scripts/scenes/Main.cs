using Enum;
using Godot;
using System;

public partial class Main : BaseScene
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var button = GetNode<Button>("button");
		button.Pressed += ClickStart;
	}

	private void ClickStart () {
		var util = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		util.LoadScene(SceneId.SELECTION);	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
