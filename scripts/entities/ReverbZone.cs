using Enum;
using Godot;
using System;
using System.Text.RegularExpressions;

public partial class ReverbZone : Area2D
{
    public override void _Ready()
    {
        AddToGroup(GroupName.REVERB_ZONE);
        BodyEntered += ZoneEnterHandler;
        BodyExited += ZoneLeaveHandler;
    }
    public void SetReverb (float val) {
        var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
        gameSystem.EmitSignal(GameSystem.SignalName.SetReverb, val);
    }
    private void ZoneEnterHandler (Node2D node)
    {
        if (node.IsInGroup(GroupName.PLAYER)) {
            SetReverb(1);
        }
    }
    private void ZoneLeaveHandler (Node2D node)
    {
        if (node.IsInGroup(GroupName.PLAYER)) {
            SetReverb(0);
        }
    }
}
