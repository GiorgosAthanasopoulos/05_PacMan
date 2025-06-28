using System;
using Godot;

namespace PacMan;

public partial class Events : Node
{
    public static event Action DotEaten;
    public static event Action PowerPelletEaten;
    public static event Action CherryEaten;
    public static event Action BlinkyWakeupScoreHit;
    public static event Action PacmanDied;
    public static event Action BlinkyDied;

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

    public static void EmitBlinyDied()
    {
        BlinkyDied?.Invoke();
    }
}
