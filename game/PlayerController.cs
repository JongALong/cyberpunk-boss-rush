using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	// Variables
	[Export] public float WalkSpeed = 150.0f;
	[Export] public float RunSpeed = 500.0f;
	[Export] public float JumpSpeed = -1000.0f;
	[Export] public int Gravity = 80;
	[Export] public float DashDistance = 200f;
 	[Export] public int DashSpeed = 1000;
	[Export] public float DashTime = 0.05f;
	[Export] public float DashDuration = 0.15f;
	[Export] public float DashCooldown = 0.25f;
	[Export] public float RunActivationTime = 0.15f;
	private PlayerStats playerStats;

	// Variable Tracking
	private bool isDashing = false;
	private bool isRunning = false;
	private float dashTimer = 0f;
	private float cooldownTimer = 0f;
	private float holdTime = 0f;
	private Vector2 dashDirection = Vector2.Zero;
	private Vector2 dashStartDistance = Vector2.Zero;

	public override void _Ready()
	{
		playerStats = GetNode<PlayerStats>("PlayerStats");
	}


	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;

		HandleTimers(deltaFloat);
		Vector2 inputDirection = GetInputDirection();
		DashAndRunHandle(inputDirection, deltaFloat);

		HandleJumpInput();

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
		velocity.X = direction * WalkSpeed;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void HandleJumpInput()
	{
		// Jump should work regardless of dash state
		if (Input.IsActionJustPressed("ui_up") && IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, playerStats.JumpVelocity);
		}
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
				isRunning = false;
			}
		}

		// Cooldown timer
		if (cooldownTimer > 0)
		{
			cooldownTimer -= delta;
		}

		if (!Input.IsActionPressed("dash") && !isDashing)
		{
			isRunning = false;
			holdTime = 0;
		}
	}

	private void DashAndRunHandle(Vector2 inputDirection, float delta)
	{
		if (Input.IsActionPressed("dash") && cooldownTimer <= 0 && inputDirection != Vector2.Zero)
		{
			holdTime += delta;

			// If held long enough, activate run mode
			if (holdTime >= RunActivationTime && !isDashing)
			{
				isRunning = true;
			}

			// If just pressed and not already running, do dash
			if (Input.IsActionJustPressed("dash") && !isRunning)
			{
				StartDash(inputDirection); // Replace the old dash start logic
			}
		}
		else if (Input.IsActionJustReleased("dash"))
		{
			// If button released before run activation, do nothing
			holdTime = 0f;
			if (!isDashing)
			{
				isRunning = false;
			}
		}

		// Add this: Update dash movement if currently dashing
		if (isDashing)
		{
			UpdateDashMovement(delta);
		}
	}

	private void StartDash(Vector2 inputDirection)
	{
		isDashing = true;
		dashTimer = DashTime;
		dashDirection = inputDirection.Normalized();
		dashStartDistance = GlobalPosition; // Remember where we started
	}

	private void UpdateDashMovement(float delta)
	{
		dashTimer -= delta;
		float dashProgress = 1f - (dashTimer / DashTime);

		// Calculate exactly where we should be
		Vector2 desiredPosition = dashStartDistance + (dashDirection * DashDistance * dashProgress);

		// Check if we can move directly to that position
		var spaceState = GetWorld2D().DirectSpaceState;
		var parameters = new PhysicsPointQueryParameters2D();
		parameters.Position = desiredPosition;
		parameters.CollisionMask = 1; // Your collision layer
		parameters.Exclude = new Godot.Collections.Array<Rid> { GetRid() };

		var result = spaceState.IntersectPoint(parameters);

		if (result.Count == 0)
		{
			// No collision, move directly
			GlobalPosition = desiredPosition;
		}
		else
		{
			// Hit something, stop dash early
			EndDash();
			return;
		}

		// Update velocity for visual consistency
		Velocity = (desiredPosition - GlobalPosition) / delta;

		if (dashTimer <= 0)
		{
			EndDash();
		}
	}

	private void EndDash()
	{
		isDashing = false;
		cooldownTimer = DashCooldown;

		// Snap to final position but only if we're close
		Vector2 exactEndPosition = dashStartDistance + (dashDirection * DashDistance);
		float distanceToTarget = (exactEndPosition - GlobalPosition).Length();

		if (distanceToTarget < 10f) // Only snap if we're within 10 pixels
		{
			GlobalPosition = exactEndPosition;
		}

		// Optional: Carry some momentum
		Velocity = dashDirection * (DashDistance / DashTime) * 0.3f;

		GD.Print($"Dash completed! Final distance: {(GlobalPosition - dashStartDistance).Length()}");
	}

	private void ApplyDashMovement()
	{
		Velocity = new Vector2(dashDirection.X * DashSpeed, Velocity.Y);
	}

	private void ApplyNormalMovement(Vector2 inputDirection, float delta)
	{
		float currentSpeed = isRunning ? RunSpeed : WalkSpeed;
		
		Velocity = new Vector2(0, Velocity.Y);
		
		if (inputDirection != Vector2.Zero)
		{
			Velocity = new Vector2(inputDirection.X * currentSpeed, Velocity.Y);
		}
	}

	private void ApplyGravity(float delta)
	{
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * delta);
		}
	}
}
