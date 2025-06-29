using System;
using Godot;

namespace PacMan;

// TODO: sometimes the ghosts get desync wearing scared/wearing times
// TODO: prob need to remove ghost layer from ghost scene cuz they collide with each other

public partial class Ghost : Character
{
    private AnimatedSprite2D GhostSprite, Eyes;

    private double ScaredInterval = 6.5f;
    protected double scaredTime = 0.0f;
    protected bool justScared = false;

    private double WearingInterval = 3.0f;
    protected double wearingTime = 0.0f;
    protected bool justWearing = false;

    protected AStarGrid2D aStarGrid = new();

    protected Node2D Pacman;
    protected TileMapLayer Map;
    protected bool enabled = false;

    [Export]
    protected Corner ScatterCorner;

    [Export]
    public double MoveInterval, MoveIntervalScared;

    [Export]
    protected GhostType ghostType;

    private String MovingAnimation, IdleAnimation;

    public override void _Ready()
    {
        base._Ready();

        GhostSprite ??= GetNode<AnimatedSprite2D>("Ghost");
        Eyes ??= GetNode<AnimatedSprite2D>("Eyes");

        Events.PowerPelletEaten += () =>
        {
            justScared = true;
            scaredTime = ScaredInterval;
        };

        Godot.Collections.Array<Node> pacmans = GetTree().GetNodesInGroup("Pacman");
        if (pacmans.Count == 0)
            GD.PushError("Ghost.cs: No Pacman found in `Pacman` group.");
        else if (pacmans.Count > 1)
            GD.PushError("Ghost.cs: Found multiple Pacmans in `Pacman` group.");
        else
            Pacman = (Node2D)pacmans[0];

        Godot.Collections.Array<Node> maps = GetTree().GetNodesInGroup("Map");
        if (maps.Count == 0)
            GD.PushError("Ghost.cs: No Map found in `Map` group.");
        else if (pacmans.Count > 1)
            GD.PushError("Ghost.cs: Found multiple Maps in `Map` group.");
        else
            Map = (TileMapLayer)maps[0];

        Godot.Collections.Array<Vector2I> Gates = [];
        Gates.Add(new Vector2I(13, 15));
        Gates.Add(new Vector2I(14, 15));

        aStarGrid.Region = Map.GetUsedRect();
        aStarGrid.CellSize = new Vector2(16, 16);
        aStarGrid.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
        aStarGrid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        aStarGrid.Update();
        foreach (Vector2I cell in Map.GetUsedCells())
            if (!Gates.Contains(cell))
                aStarGrid.SetPointSolid(cell, IsPointSolid(cell));
        foreach (Vector2I gate in Gates)
            aStarGrid.SetPointSolid(gate, false);

        if (ghostType == GhostType.BLINKY)
        {
            ScatterCorner = Corner.TOP_LEFT;
            MoveInterval = .25f;
            MoveIntervalScared = .5f;
            ghostType = GhostType.BLINKY;
            IdleAnimation = "idle_blinky";
            MovingAnimation = "moving_blinky";
            GhostSprite.Animation = IdleAnimation;
            Events.BlinkyWakeupScoreHit += () => { enabled = true; };
        }
        if (ghostType == GhostType.INKY)
        {
            ScatterCorner = Corner.BOTTOM_LEFT;
            MoveInterval = .27f;
            MoveIntervalScared = .54f;
            ghostType = GhostType.INKY;
            IdleAnimation = "idle_inky";
            MovingAnimation = "moving_inky";
            GhostSprite.Animation = IdleAnimation;
            Events.InkyWakeupScoreHit += () => { enabled = true; };
        }
        if (ghostType == GhostType.PINKY)
        {
            ScatterCorner = Corner.TOP_RIGHT;
            MoveInterval = .25f;
            MoveIntervalScared = .5f;
            ghostType = GhostType.PINKY;
            IdleAnimation = "idle_pinky";
            MovingAnimation = "moving_pinky";
            GhostSprite.Animation = IdleAnimation;
            Events.PinkyWakeupScoreHit += () => { enabled = true; };
        }
        if (ghostType == GhostType.CLYDE)
        {
            ScatterCorner = Corner.BOTTOM_RIGHT;
            MoveInterval = .3f;
            MoveIntervalScared = .6f;
            ghostType = GhostType.CLYDE;
            IdleAnimation = "idle_clyde";
            MovingAnimation = "moving_clyde";
            GhostSprite.Animation = IdleAnimation;
            Events.ClydeWakeupScoreHit += () => { enabled = true; };
        }
    }

    public override void _PhysicsProcess(double p_delta)
    {
        if (!enabled || paused)
            return;

        HandleScared(p_delta);
        HandleAnimation();
        HandleMovement(p_delta);
    }

    protected bool IsPointSolid(Vector2I p_cell)
    {
        return (bool)Map.GetCellTileData(p_cell).GetCustomData("IsSolid");
    }

    protected void HandleScared(double p_delta)
    {
        if (scaredTime > 0)
        {
            scaredTime -= p_delta;

            if (scaredTime <= 0)
            {
                wearingTime = WearingInterval;
                justWearing = true;
            }

            return;
        }

        if (wearingTime > 0)
        {
            wearingTime -= p_delta;
            return;
        }
    }

    protected void HandleEyesAnimation()
    {
        if (wearingTime > 0 || scaredTime > 0)
            Eyes.Animation = "scared";
        else
            Eyes.Animation = "default";

        if (direction == Direction.NONE)
            Eyes.Frame = 3;
        else
            Eyes.Frame = (int)direction - 1;
    }

    protected void HandleAnimation()
    {
        HandleEyesAnimation();

        if (justScared)
        {
            if (direction != Direction.NONE)
                GhostSprite.Animation = "moving_scared";
            else
                GhostSprite.Animation = "idle_scared";

            GhostSprite.Play();
            justScared = false;
            return;
        }

        if (justWearing)
        {
            if (direction != Direction.NONE)
                GhostSprite.Animation = "moving_wearing";
            else
                GhostSprite.Animation = "idle_wearing";

            GhostSprite.Play();
            justWearing = false;
            return;
        }

        if (scaredTime <= 0 && wearingTime <= 0)
        {
            if (direction != Direction.NONE)
                GhostSprite.Animation = MovingAnimation;
            else
                GhostSprite.Animation = IdleAnimation;

            GhostSprite.Play();
        }
    }

    protected void HandleMovement(double p_delta)
    {
        moveTimer -= p_delta;
        if (moveTimer > 0.0)
            return;

        HandleNavigation();

        moveTimer = scaredTime > 0.0 ? MoveIntervalScared : MoveInterval;
        if (CanMoveDirection(false))
            MoveDirection();
    }

    protected static Vector2I GlobalToIdPos(Vector2 p_pos)
    {
        return (Vector2I)p_pos / 16;
    }

#pragma warning disable IDE1006 // Naming Styles
    protected void _on_area_2d_body_entered(Node2D p_body)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (p_body.IsInGroup("Pacman"))
            if (scaredTime > 0.0)
            {
                Audio.PlaySFX(Audio.EatGhost);
                //  TODO: spawn eyes from current location and go to pen after that respawn ghost
                // NOTE: eyes should go faster than the ghost
                if (ghostType == GhostType.BLINKY)
                    Events.EmitBlinkyDied();
                else if (ghostType == GhostType.INKY)
                    Events.EmitInkyDied();
                else if (ghostType == GhostType.PINKY)
                    Events.EmitPinkyDied();
                else if (ghostType == GhostType.CLYDE)
                    Events.EmitClydeDied();
                QueueFree();
            }
            else
            {
                if (ghostType == GhostType.BLINKY)
                    GlobalPosition = new Vector2(14, 14) * 16 + new Vector2(8, 8);
                else if (ghostType == GhostType.INKY)
                    GlobalPosition = new Vector2(11, 18) * 16 + new Vector2(8, 8);
                else if (ghostType == GhostType.PINKY)
                    GlobalPosition = new Vector2(14, 18) * 16 + new Vector2(8, 8);
                else if (ghostType == GhostType.CLYDE)
                    GlobalPosition = new Vector2(16, 18) * 16 + new Vector2(8, 8);
                Events.EmitPacmanDied();
            }
    }

    protected Vector2I ComputeNextPosition()
    {
        Vector2I pacmanPos = GlobalToIdPos(Pacman.GlobalPosition);

        if (ghostType == GhostType.PINKY) { } // TODO: implement pinky nav
        if (ghostType == GhostType.INKY) { } // TODO: implement inky nav
        if (ghostType == GhostType.CLYDE) { } // TODO: implement clyde nav

        return pacmanPos;
    }

    private void HandleNavigation()
    {
        if (!IsInstanceValid(Pacman))
        {
            direction = Direction.NONE;
            return;
        }

        Vector2I current = GlobalToIdPos(GlobalPosition);
        Vector2I target = ComputeNextPosition();

        if (scaredTime > 0)
            if (ScatterCorner == Corner.TOP_LEFT)
                target = new(1, 4);
            else if (ScatterCorner == Corner.TOP_RIGHT)
                target = new(26, 4);
            else if (ScatterCorner == Corner.BOTTOM_LEFT)
                target = new(1, 33);
            else if (ScatterCorner == Corner.BOTTOM_RIGHT)
                target = new(26, 33);

        if (current == target)
        {
            direction = Direction.NONE;
            return;
        }

        Godot.Collections.Array<Vector2I> path = aStarGrid.GetIdPath(current, target, true).Slice(1);
        if (path.Count == 0)
        {
            direction = Direction.NONE;
            return;
        }

        Vector2I next = path[0];
        if (next.X - current.X > 0)
            direction = Direction.RIGHT;
        else if (next.X - current.X < 0)
            direction = Direction.LEFT;
        else if (next.Y - current.Y > 0)
            direction = Direction.DOWN;
        else if (next.Y - current.Y < 0)
            direction = Direction.UP;
        else
            direction = Direction.NONE;
    }

    protected enum GhostType
    {
        BLINKY,
        PINKY,
        INKY,
        CLYDE
    }

    public enum Corner
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }
}
