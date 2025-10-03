using Godot;

public partial class Enemy : Area2D
{
	[Export] public int Damage = 10;

	public override void _Ready()
	{
		// Connect the signal
		BodyEntered += OnBodyEntered;
		
		GD.Print("Area2D Enemy ready! Let's see some collisions!");
	}

	private void OnBodyEntered(Node2D body)
	{
		GD.Print($"AREA2D ENEMY: Collision detected with {body.Name}");
		
		// Check if it's the player
		if (body.Name == "Player" || body.IsInGroup("player"))
		{
			GD.Print("AREA2D ENEMY: Hit the player! Applying damage...");
			
			var playerStats = body.GetNodeOrNull<PlayerStats>("PlayerStats");
			if (playerStats != null)
			{
				playerStats.TakeDamage(Damage);
				GD.Print("AREA2D ENEMY: Damage applied successfully!");
			}
			else
			{
				GD.PrintErr("AREA2D ENEMY: Could not find PlayerStats!");
			}
		}
		else
		{
			GD.Print($"AREA2D ENEMY: Collided with {body.Name} (not player)");
		}
	}
}
