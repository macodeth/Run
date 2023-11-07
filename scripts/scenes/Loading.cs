using Enum;
using Godot;
using System;

public partial class Loading : BaseScene
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var util = GetNode<NodeUtil>("/root/NodeUtil");
		util.LoadScene(SceneId.MAIN);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
