using Godot;
using System;

public partial class PlayerStats : Node
{
	[Signal]
	public delegate void HealthChangedEventHandler(int CurrentHealth, int MaxHealth);
	[Signal]
	public delegate void StatChangeEventHandler();
	[Signal]
	public delegate void DiedEventHandler();

	// Health Stat
	public int MaxHealth = 100;
	public int CurrentHealth { get; private set; }

	//Dash Stat
	public int DashCharges = 1;
	public int CurrentDashes { get; private set; }

	// Player Level
	public int Level = 1;

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
		CurrentDashes = DashCharges;
	}

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;
		CurrentHealth = Mathf.Max(CurrentHealth, 0);

		EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

		if (CurrentHealth <= 0)
		{
			EmitSignal(SignalName.Died);
		}
	}



}
