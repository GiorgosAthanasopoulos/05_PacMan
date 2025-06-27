using Godot;

namespace PacMan;

public partial class Cherry : Area2D
{
#pragma warning disable IDE1006 // Naming Styles
    public static void _on_body_entered(Node2D body)
    {
        Events.EmitCherryEaten();
    }
#pragma warning restore IDE1006 // Naming Styles
}
