using Godot;

namespace PacMan;

public partial class PowerPellet : Area2D
{
#pragma warning disable IDE1006 // Naming Styles
    public void _on_body_entered(Node2D p_body)
    {
        // TODO: play power pellet eaten sfx
        Events.EmitPowerPelletEaten();
        QueueFree();
    }
#pragma warning restore IDE1006 // Naming Styles
}
