using Enum;
using Godot;
using System;

enum FruitType {
	APPLE,
	BANANA,
	CHERRIES,
	KIWI,
	MELON,
	ORANGE,
	PINEAPPLE,
	STRAWBERRY
}

public partial class Fruit : Node
{
	[Export(PropertyHint.Enum, "APPLE:0, BANANA:1, CHERRIES:2, KIWI:3, MELON:4, ORANGE:5, PINEAPPLE:6, STRAWBERRY:7")]
	private FruitType Type;
	[Export]
	public int Score = 5;
	private AnimatedSprite2D _sprite;
	private string Animation () {
		switch (Type) {
			case FruitType.APPLE:
				return "Apple";
			case FruitType.BANANA:
				return "Banana";
			case FruitType.CHERRIES:
				return "Cherries";
			case FruitType.KIWI:
				return "Kiwi";
			case FruitType.MELON:
				return "Melon";
			case FruitType.ORANGE:
				return "Orange";
			case FruitType.PINEAPPLE:
				return "Pineapple";
			case FruitType.STRAWBERRY:
				return "Strawberry";
			default:
				return "Apple";
		}
	}
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Sprite");
		_sprite.Play(Animation());
		AddToGroup(GroupName.FRUIT);
	}
}
