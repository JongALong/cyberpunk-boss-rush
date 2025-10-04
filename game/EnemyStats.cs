using Godot;
using System;

public partial class EnemyStats : Node
{
	// Signals
	[Signal] public delegate void EnemyHealthEventHandler(int CurrentEnemyHealth, int EnemyHealth);
	[Signal] public delegate void EnemyDiedEventHandler();

	// Variablees
	[Export] public int MaxEnemyHealth = 100;
	public int CurrentEnemyHealth { get; private set; }

	public override void _Ready()
	{
		CurrentEnemyHealth = MaxEnemyHealth;
	}

	public void TakeDamage(int damage)
	{
		CurrentEnemyHealth -= damage;
		CurrentEnemyHealth = Mathf.Max(CurrentEnemyHealth, 0);

		EmitSignal(SignalName.EnemyHealth, CurrentEnemyHealth, MaxEnemyHealth);

		if (CurrentEnemyHealth <= 0)
		{
			EmitSignal(SignalName.EnemyDied);
		}
	}

	public void EnemyHealing()
	{
		// Restore health over time
	}

}
