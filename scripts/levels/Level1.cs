using System;
using Godot;

namespace PacMan;

// TODO: after eating enough dots spawn 2 special powerups depending on level
// NOTE: they disappear after a while if you dont eat them (ask gpt how long they stay)
//       - strawberry 2
//       - orange 3,4
//       - apple 5,6
//       - melon 7,8
//       - galaxian 9,10
//       - bell 11,12
//       - key 13+

// TODO: play intro sfx
// TODO: play interlevel intro sfx
// TODO: play bgm
// TODO: ghosts should be able to go through white gates (pacman shouldn't)

public partial class Level1 : Node2D
{
    [Export]
    public Label score1Label, score2Label, highScoreLabel;
    private int score1 = 0, score2 = 0, highScore = 0;
    [Export]
    public int DotEatenScore = 10, PowerPelletEatenScore = 50, CherryEatenScore = 100;
    [Export]
    public int StrawberryEatenScore = 300, OrangeEatenScore = 500, AppleEatenScore = 700,
                MelonEatenScore = 1000, GalaxianEatenScore = 2000, BellEatenScore = 3000, KeyEatenScore = 5000;
    [Export]
    public int BlinkyWakeupScore = 20, PinkyWakeupScore = 100, InkyWakeupScore = 200, ClydeWakeupScore = 400; // TODO: check pinky wakeup score with gpt
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

    private bool paused = false;
    [Export]
    public CanvasLayer PauseMenu;

    public override void _Ready()
    {
        Events.DotEaten += () =>
        {
            IncreaseScore(DotEatenScore);

            Godot.Collections.Array<Node> dots = GetTree().GetNodesInGroup(DotGroup);
            if (dots.Count == 1) // queue free after event emittion
            {
                CheckHighScore();
                GetTree().ReloadCurrentScene();
                // TODO: do smth when winning? (sound/visual)
            }
        };
        Events.PowerPelletEaten += () => { IncreaseScore(PowerPelletEatenScore); };
        Events.CherryEaten += () => { IncreaseScore(CherryEatenScore); };
        Events.PacmanDied += () => { UpdateLifes(-1); };
        Events.BlinkyDied += () =>
        {
            // TODO: increase score based on how many ghosts have died in current scared state
        };
        Events.PinkyDied += () =>
        {
            // TODO: increase score based on how many ghosts have died in current scared state
        };
        Events.InkyDied += () =>
        {
            // TODO: increase score based on how many ghosts have died in current scared state
        };
        Events.ClydeDied += () =>
        {
            // TODO: increase score based on how many ghosts have died in current scared state
        };

        Settings.LoadSettings();
        SetHighScore(Settings.HighScore);

        lifes = MaxLifes;

        Life1 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter");
        Life2 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter2");
        Life3 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter3");

        PauseMenu = GetNode<CanvasLayer>("PausedMenu");

        Events.Unpaused += () =>
        {
            if (paused)
                TogglePause();
        };

        // TODO: after restarting, these are not null but at the same time they are dead(discarded) values
        score1Label ??= GetNode<Label>("UI/ScoreRow/GridContainer/1UpLabel");
        highScoreLabel ??= GetNode<Label>("UI/ScoreRow/GridContainer2/HighScoreLabel");
        score2Label ??= GetNode<Label>("UI/ScoreRow/GridContainer3/2UpLabel");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
            TogglePause();
    }

    private void TogglePause()
    {
        paused = !paused;
        PauseMenu.Visible = paused;
        if (paused)
            Events.EmitPaused();
        else
            Events.EmitUnpaused();
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
                Events.EmitBlinkyWakeupScoreHit();
            if (score1 >= PinkyWakeupScore)
                Events.EmitPinkyWakeupScoreHit();
            if (score1 >= InkyWakeupScore)
                Events.EmitInkyWakeupScoreHit();
            if (score1 >= ClydeWakeupScore)
                Events.EmitClydeWakeupScoreHit();
            if (score1 >= ExtraLifeScoreThreshold)
                UpdateLifes(1);
        }
        else if (p_player == 2)
        {
            score2 += p_scoreToAdd;
            score2Label.Text = score2.ToString();

            if (score2 >= BlinkyWakeupScore)
                Events.EmitBlinkyWakeupScoreHit();
            if (score1 >= PinkyWakeupScore)
                Events.EmitPinkyWakeupScoreHit();
            if (score2 >= ExtraLifeScoreThreshold)
                Events.EmitInkyWakeupScoreHit();
            if (score2 >= ClydeWakeupScore)
                Events.EmitClydeWakeupScoreHit();
            UpdateLifes(1);
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
                GetTree().ReloadCurrentScene();
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
