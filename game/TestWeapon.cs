using Godot;
using System;

public partial class TestWeapon : Node
{
    // Variables
    [Export] public int WeaponDamage = 5;
    [Export] public float AttackSpeed = 10f;
    [Export] public PackedScene ProjectileScene;

    private bool canAttack = false;
    private float attackTime = 0f;
    private Node2D player;


    public override void _Ready()
    {
        player = GetParent<Node2D>();
    }

    public override void _Process(double delta)
    {
        float deltaFloat = (float)delta;

        if (attackTime > 0)
        {
            deltaFloat -= attackTime;

            if (attackTime <= 0)
            {
                canAttack = true;
            }
        }
    }

    public bool TryAttack()
    {
        if (!canAttack) return false;

        Vector2 mousePos = GetGetMousePosition();

        Vector2 direction = (mousePos - player.GlobalPosition).Normalized();

        SpawnPorjectile(direction);

        canAttack = false;
        attackTime = AttackSpeed;

        return true;
    }

    private void SpawnPorjectile()
    {
        
    }

}
