using Godot;
using System;

namespace PacMan;

public partial class Pinky : CharacterBody2D
{
	[Export]
	public AnimatedSprite2D Ghost, Eyes;
	[Export]
	public String IdleAnimation = "idle", IdleWearingAnimation = "idle_wearing", IdleScaredAnimation = "idle_scared";
	[Export]
	public String MovingAnimation = "moving", MovingWearingAnimation = "moving_wearing", MovingScaredAnimation = "moving_scared";
	[Export]
	public String EyesScaredAnimation = "scared", EyesDefaultAnimation = "default";

	[Export]
	Godot.Collections.Dictionary<Direction, int> SpriteRotationMoveDirection = [];

	[Export]
	public double ScaredInterval = 6.5f;
	private double scaredTime = 0.0f;
	private bool justScared = false;

	[Export]
	public double WearingInterval = 3.0f;
	private double wearingTime = 0.0f;
	private bool justWearing = false;

	[Export]
	public double MoveInterval = .25f, MoveIntervalScared = .5f;
	private double moveTimer = 0.0f;

	private Direction direction = Direction.NONE;

	private AStarGrid2D aStarGrid = new();
	[Export]
	public String SolidPropertyName = "IsSolid";

	private Node2D Pacman;
	private TileMapLayer Map;
	[Export]
	public String PacManGroup = "PacMan", MapGroup = "Map";
	private bool enabled = false;
	[Export]
	Corner ScatterCorner = Corner.TOP_RIGHT;
	[Export]
	public Vector2I TopLeftCorner = new(1, 4), TopRightCorner = new(26, 4), BottomLeftCorner = new(1, 33),
					BottomRightCorner = new(26, 33);

	enum Direction
	{
		NONE = 0,
		UP,
		LEFT,
		DOWN,
		RIGHT
	}

	enum Corner
	{
		TOP_LEFT,
		TOP_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_RIGHT,
	}

	public override void _Ready()
	{
		Ghost ??= GetNode<AnimatedSprite2D>("Ghost");
		Eyes ??= GetNode<AnimatedSprite2D>("Eyes");

		Events.PowerPelletEaten += () =>
		{
			justScared = true;
			scaredTime = ScaredInterval;
		};

		if (SpriteRotationMoveDirection.Keys.Count != 5)
		{
			SpriteRotationMoveDirection[Direction.UP] = 270;
			SpriteRotationMoveDirection[Direction.LEFT] = 180;
			SpriteRotationMoveDirection[Direction.DOWN] = 90;
			SpriteRotationMoveDirection[Direction.RIGHT] = 0;
			SpriteRotationMoveDirection[Direction.NONE] = 0;
		}

		Godot.Collections.Array<Node> pacmans = GetTree().GetNodesInGroup(PacManGroup);
		if (pacmans.Count == 0)
			GD.PushError("Pinky.cs: No Pacman found in `" + PacManGroup + "` group.");
		else if (pacmans.Count > 1)
			GD.PushError("Pinky.cs: Found multiple Pacmans in `" + PacManGroup + "` group.");
		else
			Pacman = (Node2D)pacmans[0];

		Godot.Collections.Array<Node> maps = GetTree().GetNodesInGroup(MapGroup);
		if (maps.Count == 0)
			GD.PushError("Pinky.cs: No Map found in `" + MapGroup + "` group.");
		else if (pacmans.Count > 1)
			GD.PushError("Pinky.cs: Found multiple Maps in `" + MapGroup + "` group.");
		else
			Map = (TileMapLayer)maps[0];

		aStarGrid.Region = Map.GetUsedRect();
		aStarGrid.CellSize = new Vector2(16, 16);
		aStarGrid.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
		aStarGrid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		aStarGrid.Update();
		foreach (Vector2I cell in Map.GetUsedCells())
			aStarGrid.SetPointSolid(cell, IsSpotSolid(cell));

		Events.PinkyWakeupScoreHit += () => { enabled = true; };
	}

	private bool IsSpotSolid(Vector2I p_cell)
	{
		return (bool)Map.GetCellTileData(p_cell).GetCustomData(SolidPropertyName);
	}

	public override void _PhysicsProcess(double p_delta)
	{
		if (!enabled)
			return;

		HandleScared(p_delta);
		HandleAnimation();
		HandleMovement(p_delta);
	}

	private void HandleScared(double p_delta)
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

	private void HandleEyesAnimation()
	{
		if (wearingTime > 0 || scaredTime > 0)
			Eyes.Animation = EyesScaredAnimation;
		else
			Eyes.Animation = EyesDefaultAnimation;

		if (direction == Direction.NONE)
			Eyes.Frame = 3;
		else
			Eyes.Frame = (int)direction - 1;
	}

	private void HandleAnimation()
	{
		HandleEyesAnimation();

		if (justScared)
		{
			if (direction != Direction.NONE)
				Ghost.Animation = MovingScaredAnimation;
			else
				Ghost.Animation = IdleScaredAnimation;

			Ghost.Play();
			justScared = false;
			return;
		}

		if (justWearing)
		{
			if (direction != Direction.NONE)
				Ghost.Animation = MovingWearingAnimation;
			else
				Ghost.Animation = IdleWearingAnimation;

			Ghost.Play();
			justWearing = false;
			return;
		}

		if (scaredTime <= 0 && wearingTime <= 0)
		{
			if (direction != Direction.NONE)
				Ghost.Animation = MovingAnimation;
			else
				Ghost.Animation = IdleAnimation;

			Ghost.Play();
		}
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
		Godot.Collections.Dictionary result = spaceState.IntersectRay(query);
		if (result.Count != 0)
		{
			Node2D collider = (Node2D)result["collider"];
			return collider.IsInGroup(PacManGroup);
		}
		return true;
	}

	private void HandleMovement(double p_delta)
	{
		moveTimer -= p_delta;
		if (moveTimer > 0.0)
			return;

		HandleNavigation();

		moveTimer = scaredTime > 0.0 ? MoveIntervalScared : MoveInterval;
		if (CanMoveDirection())
			MoveDirection();
	}

	// TODO: change to pinky navigation -- try to get 4 tiles in front of pacman
	private void HandleNavigation()
	{
		if (!IsInstanceValid(Pacman))
		{
			direction = Direction.NONE;
			return;
		}

		Vector2I current = GlobalToIdPos(GlobalPosition);
		Vector2I target = GlobalToIdPos(Pacman.GlobalPosition);
		if (scaredTime > 0)
			target = TopLeftCorner;
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

	private static Vector2I GlobalToIdPos(Vector2 p_pos)
	{
		return (Vector2I)p_pos / 16;
	}

	private void _on_area_2d_body_entered(Node2D p_body)
	{
		if (p_body.IsInGroup(PacManGroup))
			if (scaredTime > 0.0)
			{
				Events.EmitPinkyDied(); //  TODO: spawn eyes from current location and go to pen after that respawn pinky
				QueueFree();
			}
			else
				Events.EmitPacmanDied();
	}
}
