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

[Tool]
public partial class Fruit : Area2D
{
	private FruitType _type;
	[Export(PropertyHint.Enum, "APPLE:0, BANANA:1, CHERRIES:2, KIWI:3, MELON:4, ORANGE:5, PINEAPPLE:6, STRAWBERRY:7")]
	private FruitType Type {
		get => _type;
		set {
			_type = value;
			_sprite = GetNode<AnimatedSprite2D>("Sprite");
			_sprite.Play(Animation());
		}
	}
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
		_sprite.AnimationFinished += AnimationFinishedHandler;
		_sprite.Play(Animation());
		AddToGroup(GroupName.FRUIT);
		BodyEntered += BodyEnteredHandler;
	}
	private bool _is_collect = false;
	private void BodyEnteredHandler (Node2D body) {
		if (body.IsInGroup(GroupName.PLAYER)) {
			if (!_is_collect) {
				var gameSystem = GetNode<GameSystem>(AutoLoad.GAME_SYSTEM);
				gameSystem.EmitSignal(GameSystem.SignalName.FruitCollected, Score);
				_sprite.Play("Collected");
			}
			_is_collect = true;
		}
	}
	private void AnimationFinishedHandler () {
		var animation = _sprite.Animation;
		if (animation == "Collected") {
			QueueFree();
		}
	}
}
