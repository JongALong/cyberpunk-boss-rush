using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	public float Speed = 200.0f;

	public float JumpSpeed = -1000.0f;

	public int Gravity = 100;

	public override void _PhysicsProcess(double delta)
	{

		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity + (float)delta);
		}
		
		Vector2 velocity = Velocity;

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity = new Vector2(velocity.Y, JumpSpeed);
		}

		// Get the input direction.
		float direction = Input.GetAxis("left", "right");
		velocity.X = direction * Speed;

		Velocity = velocity;
		MoveAndSlide();
	}

}
