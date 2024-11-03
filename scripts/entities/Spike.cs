using Godot;

[Tool]
public partial class Spike : StaticBody2D
{
	private int _spike_size = 1;
	private const int SMALL_SIZE = 8;	
	[Export]
	public int SpikeSize {
		get => _spike_size;
		set {
			_spike_size = value;
			UpdateSize();
		}
	}
	[Export]
	private Sprite2D Sprite;
	[Export]
	private CollisionShape2D Shape;
	public override void _Ready()
	{
		UpdateSize();
	}
	private void UpdateSize () {
		if (Shape == null) return;
		var rectangle = Shape.Shape as RectangleShape2D;
		rectangle.Set("size", new Vector2(SMALL_SIZE * 2 * SpikeSize, rectangle.Size.Y));
		Sprite.RegionRect = new Rect2(Sprite.RegionRect.Position, new Vector2(SMALL_SIZE * SpikeSize, Sprite.RegionRect.Size.Y));
	}
}
