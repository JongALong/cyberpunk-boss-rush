using Godot;
using System;

public partial class Projectile : Area2D
{
	[Export] public float ProjectileSpeed = 1000f;
	private float _damage;
	private Vector2 _direction;

	public void Initialize(Vector2 direction, float damage)
	{
		_direction = direction;
		_damage = damage;
		Rotation = direction.Angle();
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += _direction * ProjectileSpeed * (float)delta;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Enemy"))
		{
			var enemyStats = body.GetNodeOrNull<EnemyStats>("EnemyStats");
			if (enemyStats != null)
			{
				enemyStats.TakeDamage((int)_damage);
			}
			else
			{
				foreach (Node child in body.GetChildren())
				{
					GD.Print($"  - {child.Name} ({child.GetType()})");
				}
			}
			QueueFree();
		}
	}

}
