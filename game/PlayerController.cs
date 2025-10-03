using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	// Variables
	[Export] public float Speed = 200.0f;
	[Export] public float JumpSpeed = -1000.0f;
	[Export] public int Gravity = 100;
	[Export] public int DashSpeed = 1000;
	[Export] public float DashDuration = 0.15f;
	[Export] public float DashCooldown = 0.5f;
	private PlayerStats playerStats;

	// Variable Tracking
	private bool isDashing = false;
	private float dashTimer = 0f;
	private float cooldownTimer = 0f;
	private Vector2 dashDirection = Vector2.Zero;

	public override void _Ready()
	{
		playerStats = GetNode<PlayerStats>("PlayerStats");
		GD.Print("PlayerStats reference: " + (playerStats != null ? "FOUND" : "NULL"));
	}


	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;

		HandleTimers(deltaFloat);

		Vector2 inputDirection = GetInputDirection();

		HandleDashInput(inputDirection);

		if (isDashing)
		{
			ApplyDashMovement();
		}
		else
		{
			ApplyNormalMovement(inputDirection, deltaFloat);
		}

		MoveAndSlide();

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

	private Vector2 GetInputDirection()
	{
		Vector2 direction = Vector2.Zero;

		if (Input.IsActionPressed("right"))
			direction.X += 1;
		if (Input.IsActionPressed("left"))
			direction.X -= 1;

		return direction.Normalized(); // Normalize so diagonal dashes aren't faster
	}

	private void HandleTimers(float delta)
	{
		// Dash duration timer
		if (isDashing)
		{
			dashTimer -= delta;
			if (dashTimer <= 0)
			{
				isDashing = false;
				cooldownTimer = DashCooldown;
			}
		}

		// Cooldown timer
		if (cooldownTimer > 0)
		{
			cooldownTimer -= delta;
		}
	}

	private void HandleDashInput(Vector2 inputDirection)
	{
		// Check for dash input (Shift key by default)
		if (Input.IsActionJustPressed("dash") && cooldownTimer <= 0 && inputDirection != Vector2.Zero)
		{
			isDashing = true;
			dashTimer = DashDuration;
			dashDirection = new Vector2(inputDirection.X, 0).Normalized();
			if (dashDirection == Vector2.Zero)
			{
				dashDirection = new Vector2(GetNode<Sprite2D>("Sprite2D").FlipH ? -1 : 1, 0);
			}
		}
	}

	private void ApplyDashMovement()
	{
		Velocity = dashDirection * DashSpeed;
	}

	private void ApplyNormalMovement(Vector2 inputDirection, float delta)
	{
		// Reset horizontal velocity
		Velocity = new Vector2(0, Velocity.Y);

		// Apply horizontal movement
		if (inputDirection != Vector2.Zero)
		{
			Velocity = new Vector2(inputDirection.X * Speed, Velocity.Y);
		}
	}

	private void ApplyGravity(float delta)
	{
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * delta);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept")) // Space bar
		{
			playerStats.TakeDamage(10);
			GD.Print("Manual test: Took 10 damage. Health should be: " + playerStats.CurrentHealth);
		}
	}

}
