using System;
using Godot;

namespace PacMan;

public partial class Pacman : Character
{
    [Export]
    public float MoveInterval = .2f;
    private AnimatedSprite2D animatedSprite;

    public override void _Ready()
    {
        base._Ready();

        AddToGroup("Pacman");

        animatedSprite ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        Events.PacmanDied += () =>
        {
            Audio.PlaySFX(Audio.Death);
            GlobalPosition = new Vector2(14, 26) * 16 + new Vector2(8, 8);
            direction = Direction.NONE;
            animatedSprite.Animation = "idle";
        };
    }

    public override void _PhysicsProcess(double p_delta)
    {
        if (paused || IsQueuedForDeletion())
            return;

        HandleInput();
        ProcessInput(p_delta, MoveInterval);
        HandleAnimation();
    }

    private void HandleInput()
    {
        if (Input.IsActionJustPressed("move_up"))
            direction = Direction.UP;
        if (Input.IsActionJustPressed("move_left"))
            direction = Direction.LEFT;
        if (Input.IsActionJustPressed("move_down"))
            direction = Direction.DOWN;
        if (Input.IsActionJustPressed("move_right"))
            direction = Direction.RIGHT;
    }

    private void HandleAnimation()
    {
        bool should_start_moving_animation = direction != Direction.NONE && animatedSprite.Animation == "idle";
        if (should_start_moving_animation)
        {
            animatedSprite.Animation = "moving";
            animatedSprite.Play();
        }

        if (direction == Direction.UP)
            animatedSprite.RotationDegrees = 270;
        if (direction == Direction.LEFT)
            animatedSprite.RotationDegrees = 180;
        if (direction == Direction.DOWN)
            animatedSprite.RotationDegrees = 90;
        if (direction == Direction.RIGHT)
            animatedSprite.RotationDegrees = 0;
        if (direction == Direction.NONE)
            animatedSprite.RotationDegrees = 0;
    }
}
