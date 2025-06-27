using Godot;

namespace PacMan;

public partial class Tunnel : Node
{
    [Export]
    public bool IsLeftTunnel = true;
    [Export]
    public int RightTeleportPosition = 28 * 16 - 8 - 16 * 2; // two tiles left of right tunnel
    [Export]
    public int LeftTeleportPosition = 8 + 16 * 2; // two tiles right of left tunnel

#pragma warning disable IDE1006 // Naming Styles
    public void _on_body_entered(Node2D body)
    {
        body.GlobalPosition = body.GlobalPosition with { X = IsLeftTunnel ? RightTeleportPosition : LeftTeleportPosition };
    }
#pragma warning restore IDE1006 // Naming Styles
}
