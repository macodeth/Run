using Enum;
using Godot;
using System;

public partial class Saw : Area2D
{
	protected Sprite2D _sprite;
	private RefCounted audioEventInstance;
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		BodyEntered += BodyEnterHandler;
		var audioArea = GetNode<Area2D>("AudioArea");
		audioArea.BodyEntered += AudioEnterHandler;
		audioArea.BodyExited += AudioExitHandler;
		var collider = GetNode<CollisionShape2D>("AudioArea/Collider");
	}
	private Player player = null;
	private void AudioEnterHandler (Node2D node)
	{
		if (node.IsInGroup(GroupName.PLAYER)) {
			player = node as Player;
		}
	}
	private void AudioExitHandler (Node2D node)
	{
		if (node.IsInGroup(GroupName.PLAYER)) {
			player = null;
		}
	}
	public override void _Process(double delta)
	{
		_sprite.Rotation += (float)delta * 5;
		if (player != null) {
			Vector2 pos = player.GlobalPosition;
			Vector2 sawPos = GlobalPosition;
			var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
			gameSystem.EmitSignal(GameSystem.SignalName.PlayAudioLoop, "Saw", GetInstanceId());
			float distance = pos.DistanceTo(sawPos);
			gameSystem.EmitSignal(GameSystem.SignalName.SetDistance, GetInstanceId(), distance);
		}
	}
	private void BodyEnterHandler (Node2D body)
	{
		if (body.IsInGroup(GroupName.PLAYER)) {
			var player = body as Player;
			player.InstantDeath();
		}
	}

}
