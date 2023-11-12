using Enum;
using Godot;
public partial class Box : StaticBody2D
{
	private AnimatedSprite2D _anim;
	private const int HITPOINT = 3;
	private int HitPoint = 0;
    public override void _Ready()
    {
		HitPoint = HITPOINT;
		_anim = GetNode<AnimatedSprite2D>("sprite");
		_anim.AnimationFinished += AnimationFinishedHandler;
		AddToGroup(GroupName.TOP_BREAKABLE);
		Idle();
    }

	public void Idle () {
		_anim.Play("Idle");
	}
	public void Hit () {
		if (_anim.Animation == "Idle") {
			HitPoint -= 1;
			_anim.Play("Hit");
		}
	}
	private void AnimationFinishedHandler () {
		string anim = _anim.Animation;
		if (anim == "Hit") {
			if (HitPoint == 0) {
				QueueFree();
			} else {
				Idle();
			}
		}
	}
}
