using Godot;
using System;

public partial class Saw : StaticBody2D
{
	protected Sprite2D _sprite;
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_sprite.Rotation += (float)delta;
	}
}
