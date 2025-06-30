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

// TODO: play interlevel intro sfx (requires defining intermissions/levels)
// TODO: if pacman takes too much time to eat the dots, all ghost should wake up

public partial class Level1 : Node2D
{
    private Label score1Label, score2Label, highScoreLabel;
    private int score1 = 0, score2 = 0, highScore = 0;
    [Export]
    public int CherrySpawnScore = 10 * 10;
    [Export]
    public int DotEatenScore = 10, PowerPelletEatenScore = 50, CherryEatenScore = 100;
    [Export]
    public int StrawberryEatenScore = 300, OrangeEatenScore = 500, AppleEatenScore = 700,
                MelonEatenScore = 1000, GalaxianEatenScore = 2000, BellEatenScore = 3000, KeyEatenScore = 5000;
    [Export]
    public int BlinkyWakeupScore = 1 * 10, PinkyWakeupScore = 5 * 10, InkyWakeupScore = 30 * 10, ClydeWakeupScore = 60 * 10;
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
    private int ghostsEaten = 0;
    private double pelletModeTimer = 0.0f;

    private bool paused = false;
    [Export]
    public CanvasLayer PauseMenu;

    public override void _Ready()
    {
        score1Label = GetNode<Label>("UI/ScoreRow/GridContainer/1UpLabel");
        highScoreLabel = GetNode<Label>("UI/ScoreRow/GridContainer2/HighScoreLabel");
        score2Label = GetNode<Label>("UI/ScoreRow/GridContainer3/2UpLabel");

        Life1 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter");
        Life2 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter2");
        Life3 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter3");

        PauseMenu = GetNode<CanvasLayer>("PausedMenu");

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
        Events.PowerPelletEaten += () => { IncreaseScore(PowerPelletEatenScore); pelletModeTimer = 6.5f; };
        Events.CherryEaten += () => { IncreaseScore(CherryEatenScore); };

        Events.PacmanDied += () => { UpdateLifes(-1); }; // TODO: small pause before game start
        Events.BlinkyDied += () => { OnGhostDied(); };
        Events.PinkyDied += () => { OnGhostDied(); };
        Events.InkyDied += () => { OnGhostDied(); };
        Events.ClydeDied += () => { OnGhostDied(); };

        lifes = MaxLifes;
        UpdateLifes(1);

        Events.Unpaused += () =>
        {
            if (paused)
                TogglePause();
        };

        Settings.LoadSettings();
        SetHighScore(Settings.HighScore);

        Audio.PlayBGM(Audio.Intro);
    }

    private void OnGhostDied()
    {
        ghostsEaten++;
        if (ghostsEaten == 1)
            IncreaseScore(OneGhostEatenScore);
        else if (ghostsEaten == 2)
            IncreaseScore(TwoGhostsEatenScore);
        else if (ghostsEaten == 3)
            IncreaseScore(ThreeGhostsEatenScore);
        else if (ghostsEaten >= 4)
        {
            IncreaseScore(FourGhostsEatenScore);
            ghostsEaten = 0; // reset after 4th ghost
        }
    }

    public override void _Process(double p_delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
            TogglePause();

        if (pelletModeTimer > 0.0f)
        {
            pelletModeTimer -= p_delta;
            if (pelletModeTimer <= 0.0f)
                ghostsEaten = 0;
        }
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
            {
                Audio.PlaySFX(Audio.ExtraPac);
                UpdateLifes(1);
            }

            if (score1 >= CherrySpawnScore)
            {
                SpawnCherries();
            }
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

            if (score2 >= ExtraLifeScoreThreshold)
            {
                Audio.PlaySFX(Audio.ExtraPac);
                UpdateLifes(1);
            }

            if (score2 >= CherrySpawnScore)
            {
                SpawnCherries();
            }
        }
    }

    private void SpawnCherries()
    {
        // TODO: spawn cherries at random empty locations
    }

    private void UpdateLifes(int p_life)
    {
        if (p_life != -1 && p_life != 1)
        {
            GD.PushError("Level1.cs: Tried to add/remove more than 1 life at a time!");
            return;
        }

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
        Events.EmitLeftGameScene();
    }
}
