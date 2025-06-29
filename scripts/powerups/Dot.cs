using Godot;

namespace PacMan;

public partial class Dot : Area2D
{
#pragma warning disable IDE1006 // Naming Styles
    public void _on_body_entered(Node2D p_body)
    {
        // TODO: play dot eaten sfx
        Events.EmitDotEaten();
        QueueFree();
    }
#pragma warning restore IDE1006 // Naming Styles
}
