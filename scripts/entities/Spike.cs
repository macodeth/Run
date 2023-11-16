using Godot;
using System;

public partial class Spike : Node
{
	[Export]
	public int Size = 1;
	private Sprite2D _sprite;
	private CollisionShape2D _shape;
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite");
		_shape = GetNode<CollisionShape2D>("Shape");
		var rectangle = _shape.Shape as RectangleShape2D;
		var width = _sprite.RegionRect.Size.X;
		rectangle.Set("size", new Vector2(width * 2 * Size, rectangle.Size.Y));
		_sprite.RegionRect = new Rect2(_sprite.RegionRect.Position, new Vector2(width * Size, _sprite.RegionRect.Size.Y));
	}
}
