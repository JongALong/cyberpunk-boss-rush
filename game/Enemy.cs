using Godot;

public partial class Enemy : Area2D
{
	[Export] public int Damage = 10;

	public override void _Ready()
	{
		// Connect the signal
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		// Check if it's the player
		if (body.Name == "Player" || body.IsInGroup("player"))
		{
			
			var playerStats = body.GetNodeOrNull<PlayerStats>("PlayerStats");
			if (playerStats != null)
			{
				playerStats.TakeDamage(Damage);
			}
		}
	}
}
