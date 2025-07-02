using System;
using Godot;

namespace PacMan;

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
    public double MoveInterval, MoveIntervalScared, MoveIntervalEyes = .1f;

    [Export]
    protected GhostType ghostType;

    private string MovingAnimation, IdleAnimation;

    private bool eyes = false, justEyes = false;

    private Vector2I spawnPosition;
    private Node2D blinky;

    public override void _Ready()
    {
        base._Ready();

        GhostSprite ??= GetNode<AnimatedSprite2D>("Ghost");
        Eyes ??= GetNode<AnimatedSprite2D>("Eyes");

        Events.PowerPelletEaten += cell =>
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
        else if (maps.Count > 1)
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
            spawnPosition = new Vector2I(14, 14) * 16 + new Vector2I(8, 8);
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
            spawnPosition = new Vector2I(11, 18) * 16 + new Vector2I(8, 8);
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
            spawnPosition = new Vector2I(14, 18) * 16 + new Vector2I(8, 8);
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
            spawnPosition = new Vector2I(16, 18) * 16 + new Vector2I(8, 8);
            Events.ClydeWakeupScoreHit += () => { enabled = true; };
        }

        Events.PacmanDied += () =>
        {
            GlobalPosition = spawnPosition;
            direction = Direction.NONE;
            scaredTime = 0.0f;
            justScared = false;
            wearingTime = 0.0f;
            justWearing = false;
            eyes = false;
            justEyes = false;
        };

        Godot.Collections.Array<Node> blinkys = GetTree().GetNodesInGroup("Blinky");
        if (blinkys.Count == 0)
            GD.PushError("Ghost.cs: No Blinky found in `Blinky` group.");
        else if (blinkys.Count > 1)
            GD.PushError("Ghost.cs: Found multiple Blinkys in `Blinky` group.");
        else
            blinky = (Node2D)blinkys[0];
    }

    public override void _PhysicsProcess(double p_delta)
    {
        if (!enabled || paused)
            return;

        if (eyes)
            HandleEyes();
        else
        {
            HandleScared(p_delta);
            HandleAnimation();
        }
        HandleMovement(p_delta);
    }

    private void HandleEyes()
    {
        if (!justEyes)
            return;

        justEyes = false;

        GhostSprite.Visible = false;

        scaredTime = 0.0f;
        justScared = false;

        wearingTime = 0.0f;
        justWearing = false;
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

        moveTimer = MoveInterval;
        if (scaredTime > 0.0)
            moveTimer = MoveIntervalScared;
        else if (eyes)
            moveTimer = MoveIntervalEyes;

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
        if (eyes)
            return;

        if (p_body.IsInGroup("Pacman"))
            if (scaredTime > 0.0)
            {
                Audio.PlaySFX(Audio.EatGhost);
                if (ghostType == GhostType.BLINKY)
                    Events.EmitBlinkyDied();
                else if (ghostType == GhostType.INKY)
                    Events.EmitInkyDied();
                else if (ghostType == GhostType.PINKY)
                    Events.EmitPinkyDied();
                else if (ghostType == GhostType.CLYDE)
                    Events.EmitClydeDied();
                eyes = true;
                justEyes = true;
            }
            else
                Events.EmitPacmanDied();
    }

    protected Vector2I ComputeNextPosition()
    {
        if (eyes)
        {
            if (GlobalPosition == spawnPosition)
            {
                eyes = false;
                GhostSprite.Visible = true;
            }

            return GlobalToIdPos(spawnPosition);
        }

        Vector2I pacmanPos = GlobalToIdPos(Pacman.GlobalPosition);

        if (ghostType == GhostType.PINKY)
        {
            if (Pacman.GlobalRotationDegrees == 270)  // up
                return pacmanPos - new Vector2I(0, 4);
            else if (Pacman.GlobalRotationDegrees == 180) // left
                return pacmanPos - new Vector2I(4, 0);
            else if (Pacman.GlobalRotationDegrees == 90)  // down
                return pacmanPos + new Vector2I(0, 4);
            else if (Pacman.GlobalRotationDegrees == 0) // right/none
                return pacmanPos + new Vector2I(4, 0);
        }

        if (ghostType == GhostType.INKY)
        {
            Vector2I pacman2Ahead = GlobalToIdPos(Pacman.GlobalPosition);
            if (Pacman.GlobalRotationDegrees == 270)  // up
                pacman2Ahead -= new Vector2I(0, 2);
            if (Pacman.GlobalRotationDegrees == 180) // left
                pacman2Ahead -= new Vector2I(2, 0);
            if (Pacman.GlobalRotationDegrees == 90)  // down
                pacman2Ahead += new Vector2I(0, 2);
            if (Pacman.GlobalRotationDegrees == 0) // right/none
                pacman2Ahead += new Vector2I(2, 0);

            Vector2I blinkyPos = GlobalToIdPos(blinky.GlobalPosition);
            Vector2I vector2 = pacman2Ahead - blinkyPos;
            return blinkyPos + vector2 * 2;
        }

        if (ghostType == GhostType.CLYDE)
        {
            int pacmanClydeDistance = (int)Math.Ceiling((GlobalToIdPos(Pacman.GlobalPosition) - GlobalToIdPos(GlobalPosition)).Length());
            if (pacmanClydeDistance > 8)
                return pacmanPos;
            else
                return new Vector2I(16, 18);
        }

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
        if (!aStarGrid.IsInBounds(target.X, target.Y))
        {
            GD.PushError("nav target out of bounds!");
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
        CLYDE,
    }

    public enum Corner
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }
}
