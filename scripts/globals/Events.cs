using System;
using Godot;

namespace PacMan;

public partial class Events : Node
{
    public static event Action<Vector2I> DotEaten;
    public static event Action<Vector2I> PowerPelletEaten;
    public static event Action<Vector2I> CherryEaten;
    public static event Action<Vector2I> CherryExpired;

    public static event Action BlinkyWakeupScoreHit;
    public static event Action BlinkyDied;

    public static event Action PinkyDied;
    public static event Action PinkyWakeupScoreHit;

    public static event Action InkyDied;
    public static event Action InkyWakeupScoreHit;

    public static event Action ClydeDied;
    public static event Action ClydeWakeupScoreHit;

    public static event Action PacmanDied;

    public static event Action Paused;
    public static event Action Unpaused;

    public static event Action LeftGameScene;

    public static void EmitCherryExpired(Vector2I p_cell)
    {
        CherryExpired?.Invoke(p_cell);
    }

    public static void EmitDotEaten(Vector2I p_cell)
    {
        DotEaten?.Invoke(p_cell);
    }

    public static void EmitPowerPelletEaten(Vector2I p_cell)
    {
        PowerPelletEaten?.Invoke(p_cell);
    }

    public static void EmitCherryEaten(Vector2I p_cell)
    {
        CherryEaten?.Invoke(p_cell);
    }

    public static void EmitBlinkyWakeupScoreHit()
    {
        BlinkyWakeupScoreHit?.Invoke();
    }

    public static void EmitPacmanDied()
    {
        PacmanDied?.Invoke();
    }

    public static void EmitBlinkyDied()
    {
        BlinkyDied?.Invoke();
    }

    public static void EmitPinkyDied()
    {
        PinkyDied?.Invoke();
    }

    public static void EmitPinkyWakeupScoreHit()
    {
        PinkyWakeupScoreHit?.Invoke();
    }

    public static void EmitInkyDied()
    {
        InkyDied?.Invoke();
    }

    public static void EmitInkyWakeupScoreHit()
    {
        InkyWakeupScoreHit?.Invoke();
    }

    public static void EmitClydeDied()
    {
        ClydeDied?.Invoke();
    }

    public static void EmitClydeWakeupScoreHit()
    {
        ClydeWakeupScoreHit?.Invoke();
    }

    public static void EmitPaused()
    {
        Paused?.Invoke();
    }

    public static void EmitUnpaused()
    {
        Unpaused?.Invoke();
    }

    public static void EmitLeftGameScene()
    {
        LeftGameScene?.Invoke();
    }

    public override void _Ready()
    {
        Events.DotEaten = null;
        Events.PowerPelletEaten = null;
        Events.CherryEaten = null;
        Events.PacmanDied = null;
        Events.BlinkyDied = null;
        Events.PinkyDied = null;
        Events.InkyDied = null;
        Events.ClydeDied = null;
        Events.Unpaused = null;
    }
}
