using Godot;
using System;

public partial class HealthBar : Control
{
	[Export] public PlayerStats PlayerStats;

	private ColorRect Bar;
	private Label HealthNumber;

	public override void _Ready()
	{
		Bar = GetNode<ColorRect>("Bar");
		HealthNumber = GetNode<Label>("Label");

		if (PlayerStats != null)
		{
			PlayerStats.HealthChanged += OnHealthChanged;
			OnHealthChanged(PlayerStats.CurrentHealth, PlayerStats.MaxHealth);
		}
	}

	public void OnHealthChanged(int CurrentHealth, int MaxHealth)
	{
		float HealthPercentage = (float)CurrentHealth / MaxHealth;

		Bar.Size = new Vector2(200 * HealthPercentage, 30);

		HealthNumber.Text = $"{CurrentHealth}/{MaxHealth}";
	}

}
