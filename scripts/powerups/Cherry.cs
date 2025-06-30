using System.Collections;
using Godot;

namespace PacMan;

public partial class Cherry : Area2D
{
    [Export]
    public float liveTime = 10.0f;

#pragma warning disable IDE1006 // Naming Styles
    public void _on_body_entered(Node2D p_body)
    {
        Audio.PlaySFX(Audio.EatFruit);
        Events.EmitCherryEaten((Vector2I)GlobalPosition / 16);
        QueueFree();
    }
#pragma warning restore IDE1006 // Naming Styles

    public override void _Process(double delta)
    {
        liveTime -= (float)delta;
        if (liveTime <= 0.0f)
        {
            Events.EmitCherryExpired((Vector2I)GlobalPosition / 16);
            QueueFree();
        }
    }
}
