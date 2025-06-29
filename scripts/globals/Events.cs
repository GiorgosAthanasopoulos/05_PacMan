using System;
using Godot;

namespace PacMan;

public partial class Events : Node
{
    public static event Action DotEaten;
    public static event Action PowerPelletEaten;
    public static event Action CherryEaten;

    public static event Action BlinkyWakeupScoreHit;
    public static event Action BlinkyDied;

    public static event Action PinkyDied;
    public static event Action PinkyWakeupScoreHit;

    public static event Action InkyDied;
    public static event Action InkyWakeupScoreHit;

    public static event Action ClydeDied;
    public static event Action ClydeWakeupScoreHit;

    public static event Action PacmanDied;

    public static void EmitDotEaten()
    {
        DotEaten?.Invoke();
    }

    public static void EmitPowerPelletEaten()
    {
        PowerPelletEaten?.Invoke();
    }

    public static void EmitCherryEaten()
    {
        CherryEaten?.Invoke();
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
}
