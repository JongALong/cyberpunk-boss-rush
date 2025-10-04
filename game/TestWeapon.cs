using Godot;
using System;

public partial class TestWeapon : Node2D
{
	// Variables
	[Export] public int Damage = 5;
	[Export] public float AttackSpeed = 10f;
	[Export] public PackedScene ProjectileScene;

	private bool canAttack = true;
	private float attackTime = 0f;
	private Node2D player;


	public override void _Ready()
	{
		player = GetParent<Node2D>();
	}

	public override void _Process(double delta)
	{
		float deltaFloat = (float)delta;

		if (!canAttack)
		{
			attackTime -= deltaFloat;

			if (attackTime <= 0)
			{
				canAttack = true;
				attackTime = 0;
			}
		}
	}

	public bool TryAttack()
	{
		if (!canAttack)
		{
			return false;
		}

		Vector2 mousePos = GetGlobalMousePosition();
		Vector2 direction = (mousePos - player.GlobalPosition).Normalized();
		SpawnProjectile(direction);

		canAttack = false;
		attackTime = 1.0f / AttackSpeed;

		return true;
	}

	private void SpawnProjectile(Vector2 direction)
	{
		if (ProjectileScene == null)
		{
			return;
		}

		var projectile = ProjectileScene.Instantiate<Node2D>();

		projectile.GlobalPosition = player.GlobalPosition;
		projectile.Rotation = direction.Angle();

		GetTree().CurrentScene.AddChild(projectile);

		var projectileScript = projectile as Projectile;
		if (projectileScript != null)
		{
			projectileScript.Initialize(direction, Damage);
		}
	}

}
