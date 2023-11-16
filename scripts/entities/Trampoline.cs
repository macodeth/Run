using Godot;

public partial class Trampoline : MoveablePlatform
{
	protected AnimationPlayer _anim;
	protected Sprite2D _sprite;
	public override void _Ready()
	{
		base._Ready();
		_sprite = GetNode<Sprite2D>("Sprite");
		_anim = GetNode<AnimationPlayer>("Anim");
		_anim.AnimationFinished += AnimationFinishedHandler;
		_sprite.FrameChanged += FrameChangedHandler;
		Idle();
	}
	private void CheckPush () {
		foreach (var body in _bodyList) {
			if (body is Player) {
				if (_anim.CurrentAnimation == "Idle") {
					Push();
				}
			}
		}
	}
	private void FrameChangedHandler () {
		CheckPush();
	}
    private void AnimationFinishedHandler(StringName animName)
    {
		if (animName == "Push")
			Idle();
    }
	private void Idle () {
		_anim.Play("Idle");
	}
	public void Push () {
		_anim.Play("Push");
	}
	protected override void TopBodyEnteredHandler (Node2D body) {
		base.TopBodyEnteredHandler(body);
		CheckPush();
	}
}
