using System;
using Enum;
using Godot;
public partial class Box : StaticBody2D
{
	[Export]
	public int HITPOINT = 3;
	[Export(PropertyHint.Range, "1,3,")]
	public int TYPE = 1;
	private AnimatedSprite2D _anim;
	private int HitPoint = 0;
    public override void _Ready()
    {
		HitPoint = HITPOINT;
		_anim = GetNode<AnimatedSprite2D>("sprite");
		_anim.AnimationFinished += AnimationFinishedHandler;
		AddToGroup(GroupName.TOP_BREAKABLE);
		Idle();
    }
	private string HitAnim () {
		return "Hit" + TYPE;
	}
	private string IdleAnim () {
		return "Idle" + TYPE;
	}
	public void Idle () {
		_anim.Play(IdleAnim());
	}
	public void Hit () {
		if (_anim.Animation == IdleAnim()) {
			HitPoint -= 1;
			_anim.Play(HitAnim());
		}
	}
	private void AnimationFinishedHandler () {
		string anim = _anim.Animation;
		if (anim == HitAnim()) {
			if (HitPoint == 0) {
				QueueFree();
			} else {
				Idle();
			}
		}
	}
}
