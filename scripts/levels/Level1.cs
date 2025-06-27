using Godot;

namespace PacMan;

public partial class Level1 : Node2D
{
    [Export]
    public Label score1Label, score2Label, highScoreLabel;
    private int score1 = 0, score2 = 0, highScore = 0; // TODO: load/save high score
    [Export]
    public int DotEatenScore = 10, PowerPelletEatenScore = 50, CherryEatenScore = 100;

    public override void _Ready()
    {
        Events.DotEaten += OnDotEaten;
        Events.PowerPelletEaten += OnPowerPelletEaten;
        Events.CherryEaten += OnCherryEaten;
    }

    private void OnDotEaten()
    {
        score1 += DotEatenScore;
        score1Label.Text = score1.ToString();
    }

    private void OnPowerPelletEaten()
    {
        score1 += PowerPelletEatenScore;
        score1Label.Text = score1.ToString();
    }

    private void OnCherryEaten()
    {
        score1 += CherryEatenScore;
        score1Label.Text = score1.ToString();
    }
}
