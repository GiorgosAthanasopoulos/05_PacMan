using Godot;

namespace PacMan;

public partial class PowerPellet : Area2D
{
#pragma warning disable IDE1006 // Naming Styles
    public void _on_body_entered(Node2D p_body)
    {
        if (!Audio.IsPlaying(Audio.Chomp))
            Audio.PlaySFX(Audio.Chomp);
        Events.EmitPowerPelletEaten();
        QueueFree();
    }
#pragma warning restore IDE1006 // Naming Styles
}
