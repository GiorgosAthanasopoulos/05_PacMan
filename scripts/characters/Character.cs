using System;
using Godot;

namespace PacMan;

public partial class Character : CharacterBody2D
{
    protected Direction direction = Direction.NONE;
    protected bool paused = false;
    protected double moveTimer = 0.0f;

    public override void _Ready()
    {
        Events.Paused += () => { paused = true; };
        Events.Unpaused += () => { paused = false; };
    }


    protected bool CanMoveDirection(bool p_pacman)
    {
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        Vector2 target = Vector2.Zero;

        if (direction == Direction.UP)
            target = GlobalPosition + new Vector2(0, -16);
        if (direction == Direction.LEFT)
            target = GlobalPosition + new Vector2(-16, 0);
        if (direction == Direction.DOWN)
            target = GlobalPosition + new Vector2(0, 16);
        if (direction == Direction.RIGHT)
            target = GlobalPosition + new Vector2(16, 0);

        PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, target);
        Godot.Collections.Dictionary result = spaceState.IntersectRay(query);
        if (p_pacman)
            return result.Count == 0;

        if (result.Count != 0)
        {
            Node2D collider = (Node2D)result["collider"];
            return collider.IsInGroup("Pacman") || collider.IsInGroup("Gates");
        }
        return true;
    }


    protected void MoveDirection()
    {
        if (direction == Direction.UP)
            GlobalPosition = GlobalPosition with { Y = GlobalPosition.Y - 16 };
        if (direction == Direction.LEFT)
            GlobalPosition = GlobalPosition with { X = GlobalPosition.X - 16 };
        if (direction == Direction.DOWN)
            GlobalPosition = GlobalPosition with { Y = GlobalPosition.Y + 16 };
        if (direction == Direction.RIGHT)
            GlobalPosition = GlobalPosition with { X = GlobalPosition.X + 16 };
    }


    protected void ProcessInput(double p_delta, double p_moveInterval)
    {
        moveTimer -= p_delta;
        if (moveTimer > 0)
            return;

        moveTimer = p_moveInterval;
        if (CanMoveDirection(true))
            MoveDirection();
    }


    public enum Direction
    {
        NONE = 0,
        UP,
        LEFT,
        DOWN,
        RIGHT
    }
}
