using Godot;

namespace PacMan;

public partial class Cherry : Area2D
{
#pragma warning disable IDE1006 // Naming Styles
    public void _on_body_entered(Node2D p_body)
    {
        // TODO: play cherry eaten sfx
        Events.EmitCherryEaten();
        QueueFree();
    }
#pragma warning restore IDE1006 // Naming Styles
}
