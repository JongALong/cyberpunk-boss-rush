using Godot;

public partial class Enemy : Area2D
{
	[Export] public int Damage = 10;
	private EnemyStats enemyStats;

	public override void _Ready()
	{
		// Connect the signal
		BodyEntered += OnBodyEntered;
		enemyStats = GetNode<EnemyStats>("EnemyStats");
		enemyStats.EnemyDied += OnEnemyDied;
		GetNode<EnemyStats>("EnemyStats").TakeDamage(5);
	}

	// Contact damage to player
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

	// Enemy death
	private void OnEnemyDied()
	{
		QueueFree();
	}
}
