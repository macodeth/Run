using System.Collections.Generic;
using Enum;
using Godot;

public partial class MoveablePlatform : StaticBody2D {
   	private Area2D _top;
	private CollisionShape2D _collision;
	private float _prev_y = 0;
    private float _prev_x = 0;
    [Export]
	public double PROPEL_VELOCITY = -700;
	public void DisableCollision () {
		_collision.SetDeferred("disabled", true);
	}
    public override void _Ready()
    {
   		_top = GetNode<Area2D>("Top");
		_collision = GetNode<CollisionShape2D>("Shape");
		_top.BodyEntered += TopBodyEnteredHandler;
		_top.BodyExited += TopBodyExitedHandler;
    }
    public void ObjectStay () {
		_prev_y = _collision.Position.Y;
        _prev_x = _collision.Position.X;
    }
    public void ObjectMove () {
		float currentY = _collision.Position.Y;
		float diffY = currentY - _prev_y;
		_prev_y = currentY;
        float currentX = _collision.Position.X;
        float diffX = currentX - _prev_x;
        _prev_x = currentX;
		foreach (var body in _bodyList) {
			body.Position = new Vector2(body.Position.X + diffX, body.Position.Y + diffY);
		}
	}
	public void ObjectJump () {
		ObjectMove();
		foreach (var body in _bodyList) {
			if (body.IsInGroup(GroupName.PLAYER)) {
				var player = body as Player;
				player.Jump(PROPEL_VELOCITY);
			}
		}
	}	
	protected List<Node2D> _bodyList = new();
	protected virtual void TopBodyEnteredHandler (Node2D body) {
		if (body is StaticBody2D) return;
		if (_bodyList.IndexOf(body) == -1)
			_bodyList.Add(body);
	}
	protected void TopBodyExitedHandler (Node2D body) {
		_bodyList.Remove(body);
	}

}