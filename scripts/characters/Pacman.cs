using System;
using System.Collections;
using Godot;

namespace PacMan;

public partial class Pacman : CharacterBody2D
{
    // https://github.com/GiorgosAthanasopoulos/05_PacMan/blob/dccd8fd1b302144947153c10f297c07267eeff36/scripts/characters/pacman/pacman.gd

    [Export]
    public float MoveInterval = .2f;
    [Export]
    public String MoveUpAction = "move_up", MoveLeftAction = "move_left", MoveDownAction = "move_down", MoveRightAction = "move_right";
    [Export]
    public String IdleAnimation = "idle", MovingAnimation = "moving";
    [Export]
    Godot.Collections.Dictionary<Direction, int> SpriteRotationMoveDirection = [];
    [Export]
    public AnimatedSprite2D animatedSprite;

    private Direction direction = Direction.NONE;
    private double moveTimer = 0.0f;

    private bool enabled = true;

    public override void _Ready()
    {
        animatedSprite ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (SpriteRotationMoveDirection.Keys.Count != 5)
        {
            SpriteRotationMoveDirection[Direction.UP] = 270;
            SpriteRotationMoveDirection[Direction.LEFT] = 180;
            SpriteRotationMoveDirection[Direction.DOWN] = 90;
            SpriteRotationMoveDirection[Direction.RIGHT] = 0;
            SpriteRotationMoveDirection[Direction.NONE] = 0;
        }

        Events.PacmanDied += () =>
        {
            QueueFree(); // TODO: respawn in original position
        };

        Events.Paused += () =>
        {
            enabled = false;
        };
        Events.Unpaused += () =>
        {
            enabled = true;
        };
    }

    public override void _PhysicsProcess(double p_delta)
    {
        if (!enabled)
            return;

        HandleInput();
        ProcessInput(p_delta);
        HandleAnimation();
    }

    private void HandleInput()
    {
        if (Input.IsActionJustPressed(MoveUpAction))
            direction = Direction.UP;
        if (Input.IsActionJustPressed(MoveLeftAction))
            direction = Direction.LEFT;
        if (Input.IsActionJustPressed(MoveDownAction))
            direction = Direction.DOWN;
        if (Input.IsActionJustPressed(MoveRightAction))
            direction = Direction.RIGHT;
    }

    private void ProcessInput(double p_delta)
    {
        moveTimer -= p_delta;
        if (moveTimer > 0)
            return;

        // TODO: play pacman moving sfx
        moveTimer = MoveInterval;
        if (CanMoveDirection())
            MoveDirection();
    }

    private void MoveDirection()
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

    private bool CanMoveDirection()
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
        return spaceState.IntersectRay(query).Count == 0;
    }

    private void HandleAnimation()
    {
        bool should_start_moving_animation = direction != Direction.NONE && animatedSprite.Animation == IdleAnimation;
        if (should_start_moving_animation)
        {
            animatedSprite.Animation = MovingAnimation;
            animatedSprite.Play();
        }

        animatedSprite.RotationDegrees = SpriteRotationMoveDirection[direction];
    }

    enum Direction
    {
        NONE = 0,
        UP,
        LEFT,
        DOWN,
        RIGHT
    }
}
