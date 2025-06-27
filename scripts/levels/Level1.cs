using Godot;

namespace PacMan;

public partial class Level1 : Node2D
{
    [Export]
    public Label score1Label, score2Label, highScoreLabel;
    private int score1 = 0, score2 = 0, highScore = 0;
    [Export]
    public int DotEatenScore = 10, PowerPelletEatenScore = 50, CherryEatenScore = 100;

    public override void _Ready()
    {
        Events.DotEaten += OnDotEaten;
        Events.PowerPelletEaten += OnPowerPelletEaten;
        Events.CherryEaten += OnCherryEaten;

        Settings.LoadSettings();
        SetHighScore(Settings.HighScore);
    }

    private void OnDotEaten()
    {
        IncreaseScore(DotEatenScore);
    }

    private void OnPowerPelletEaten()
    {
        IncreaseScore(PowerPelletEatenScore);
    }

    private void OnCherryEaten()
    {
        IncreaseScore(CherryEatenScore);
    }

    private void IncreaseScore(int p_scoreToAdd, int p_player = 1)
    {
        if (p_player == 1)
        {
            score1 += p_scoreToAdd;
            score1Label.Text = score1.ToString();
        }
        else if (p_player == 2)
        {
            score2 += p_scoreToAdd;
            score2Label.Text = score2.ToString();
        }
    }

    private void SetHighScore(int p_highScore)
    {
        highScore = p_highScore;
        highScoreLabel.Text = highScore.ToString();
    }

    public override void _ExitTree()
    {
        Settings.SaveSettings();
    }
}
