using Enum;
using Godot;
using System;

public partial class Loading : BaseScene
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		var util = GetNode<NodeUtil>(AutoLoad.NODE_UTIL);
		util.LoadScene(SceneId.MAIN);
	}
}
