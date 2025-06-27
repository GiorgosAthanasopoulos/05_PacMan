using System;
using Godot;

namespace PacMan;

public partial class Events : Node
{
    public static event Action DotEaten;
    public static event Action PowerPelletEaten;
    public static event Action CherryEaten;

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
}
