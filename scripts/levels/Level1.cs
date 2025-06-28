using System;
using Godot;

namespace PacMan;

public partial class Level1 : Node2D
{
    [Export]
    public Label score1Label, score2Label, highScoreLabel;
    private int score1 = 0, score2 = 0, highScore = 0;
    [Export]
    public int DotEatenScore = 10, PowerPelletEatenScore = 50, CherryEatenScore = 100;
    [Export]
    public int BlinkyWakeupScore = 20;
    [Export]
    public int MaxLifes = 3;
    private int lifes;
    [Export]
    public Sprite2D Life1, Life2, Life3;
    [Export]
    public String DotGroup = "Dots";
    [Export]
    public int ExtraLifeScoreThreshold = 10_000;
    [Export]
    public int OneGhostEatenScore = 200, TwoGhostsEatenScore = 400, ThreeGhostsEatenScore = 800, FourGhostsEatenScore = 1600;

    public override void _Ready()
    {
        Events.DotEaten += () =>
        {
            IncreaseScore(DotEatenScore);

            Godot.Collections.Array<Node> dots = GetTree().GetNodesInGroup(DotGroup);
            if (dots.Count == 0)
            {
                CheckHighScore();
                GetTree().ReloadCurrentScene();
            }
        };
        Events.PowerPelletEaten += () => { IncreaseScore(PowerPelletEatenScore); };
        Events.CherryEaten += () => { IncreaseScore(CherryEatenScore); };
        Events.PacmanDied += () => { UpdateLifes(-1); };
        Events.BlinkyDied += () =>
        {
            // TODO: increase score based on how many ghosts have died in current scared state
        };

        Settings.LoadSettings();
        SetHighScore(Settings.HighScore);

        lifes = MaxLifes;

        Life1 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter");
        Life2 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter2");
        Life3 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter3");
    }

    private void CheckHighScore()
    {
        if (score1 > highScore)
            highScore = score1;
        if (score2 > highScore)
            highScore = score2;
        Settings.SaveSettings();
    }

    private void IncreaseScore(int p_scoreToAdd, int p_player = 1)
    {
        if (p_player == 1)
        {
            score1 += p_scoreToAdd;
            score1Label.Text = score1.ToString();

            if (score1 >= BlinkyWakeupScore)
            {
                Events.EmitBlinkyWakeupScoreHit();
            }
            if (score1 >= ExtraLifeScoreThreshold)
            {
                UpdateLifes(1);
            }
        }
        else if (p_player == 2)
        {
            score2 += p_scoreToAdd;
            score2Label.Text = score2.ToString();

            if (score2 >= BlinkyWakeupScore)
            {
                Events.EmitBlinkyWakeupScoreHit();
            }
        }
    }

    private void UpdateLifes(int p_life)
    {
        if (p_life != -1 && p_life != 1)
        {
            GD.PushError("Level1.cs: Tried to add/remove more than 1 life at a time!");
            return;
        }

        // TODO: update ui lifes

        if (p_life == -1)
        {
            if (lifes < 1)
            {
                GD.PushError("Level1.cs: Tried to remove life when having none!");
                return;
            }

            lifes -= 1;

            if (lifes == 0)
            {
                GetTree().ReloadCurrentScene();
            }
        }
        else if (p_life == 1)
        {
            if (lifes > MaxLifes - 1)
            {
                GD.PushError("Level1.cs: Tried to add life when having max");
                return;
            }

            lifes += 1;
        }

        Life1.Visible = lifes >= 1;
        Life2.Visible = lifes >= 2;
        Life3.Visible = lifes >= 3;
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
