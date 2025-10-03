using Godot;
using System;

public partial class Enemy : RigidBody2D
{
	public int Damage = 10;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;

		Freeze = true;
		LockRotation = true;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is PlayerController player)
		{
			GD.Print("Enemy hit Player");

			PlayerStats playerStats = player.GetNode<PlayerStats>("PlayerStats");
			if (playerStats != null)
			{
				playerStats.TakeDamage(Damage);
			}
		}
	}
}
