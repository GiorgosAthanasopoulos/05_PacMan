using System;
using System.Collections.Generic;
using Godot;

namespace PacMan;

public partial class Pacman : CharacterBody2D
{
    // https://github.com/GiorgosAthanasopoulos/05_PacMan/blob/dccd8fd1b302144947153c10f297c07267eeff36/scripts/characters/pacman/pacman.gd
    [Export]
    public float MoveInterval = .5f;
    [Export]
    public String MoveUpAction = "move_up";
    [Export]
    public String MoveLeftAction = "move_left";
    [Export]
    public String MoveDownAction = "move_down";
    [Export]
    public String MoveRightAction = "move_right";
    [Export]
    public String IdleAnimation = "idle";
    [Export]
    public String MovingAnimation = "moving";
    [Export]
    Godot.Collections.Dictionary<Direction, int> SpriteRotationMoveDirection = [];
    [Export]
    public AnimatedSprite2D animatedSprite;

    private Direction direction = Direction.NONE;
    private float moveTimer = 0.0f;

    public override void _Ready()
    {
        animatedSprite ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        SpriteRotationMoveDirection[Direction.UP] = 270;
        SpriteRotationMoveDirection[Direction.LEFT] = 180;
        SpriteRotationMoveDirection[Direction.DOWN] = 90;
        SpriteRotationMoveDirection[Direction.RIGHT] = 0;
        SpriteRotationMoveDirection[Direction.NONE] = 0;
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
