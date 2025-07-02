using Godot;

namespace PacMan;

public partial class Level1 : Node2D
{
    private Label score1Label, score2Label, highScoreLabel;
    private int score1 = 0, score2 = 0;
    [Export]
    public int CherrySpawnScore = 50 * 10;
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
    public int ExtraLifeScoreThreshold = 10_000;
    [Export]
    public int OneGhostEatenScore = 200, TwoGhostsEatenScore = 400, ThreeGhostsEatenScore = 800, FourGhostsEatenScore = 1600;
    private int ghostsEaten = 0;
    private double pelletModeTimer = 0.0f;
    private bool paused = false;
    [Export]
    public CanvasLayer PauseMenu;
    private Godot.Collections.Array<Vector2I> freePowerupPositions = [];
    [Export]
    public double ForceWakeupGhostTime = 30.0f;
    [Export]
    public PackedScene CherryScene = ResourceLoader.Load<PackedScene>("res://scenes/powerups/cherry.tscn");
    private bool spawnedCherriesThisRound = false;
    private double pauseTimer = 0.0f;
    private float pacmanRespawnPauseTime = 2.0f;

    public override void _Ready()
    {
        score1Label = GetNode<Label>("UI/ScoreRow/GridContainer/1UpLabel");
        highScoreLabel = GetNode<Label>("UI/ScoreRow/GridContainer2/HighScoreLabel");
        score2Label = GetNode<Label>("UI/ScoreRow/GridContainer3/2UpLabel");

        Life1 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter");
        Life2 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter2");
        Life3 = GetNode<Sprite2D>("UI/LifePowerupRow/GridContainer/LifeCounter3");

        PauseMenu = GetNode<CanvasLayer>("PausedMenu");

        Events.DotEaten += cell =>
        {
            IncreaseScore(DotEatenScore);
            freePowerupPositions.Add(cell);

            Godot.Collections.Array<Node> dots = GetTree().GetNodesInGroup("Dots");
            if (dots.Count == 1) // queue free after event emittion
            {
                CheckHighScore();
                GetTree().ReloadCurrentScene();
                Audio.PlaySFX(Audio.Intermission);
            }
        };
        Events.PowerPelletEaten += cell =>
        {
            IncreaseScore(PowerPelletEatenScore);
            pelletModeTimer = 6.5f;
            freePowerupPositions.Add(cell);
        };
        Events.CherryEaten += cell =>
        {
            IncreaseScore(CherryEatenScore);
            freePowerupPositions.Add(cell);
        };
        Events.CherryExpired += cell =>
        {
            freePowerupPositions.Add(cell);
        };

        Events.PacmanDied += () =>
        {
            UpdateLifes(-1);
            Events.EmitPaused();
            pauseTimer = pacmanRespawnPauseTime;
        };
        Events.BlinkyDied += () => { OnGhostDied(); };
        Events.PinkyDied += () => { OnGhostDied(); };
        Events.InkyDied += () => { OnGhostDied(); };
        Events.ClydeDied += () => { OnGhostDied(); };

        lifes = MaxLifes;

        Events.Unpaused += () =>
        {
            if (paused)
                TogglePause();
        };

        Settings.LoadSettings();
        highScoreLabel.Text = Settings.HighScore.ToString();

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

        if (paused)
            return;

        if (pauseTimer > 0.0f)
        {
            pauseTimer -= p_delta;
            if (pauseTimer < 0.0f)
                Events.EmitUnpaused();
        }

        if (pelletModeTimer > 0.0f)
        {
            pelletModeTimer -= p_delta;
            if (pelletModeTimer <= 0.0f)
                ghostsEaten = 0;
        }

        if (ForceWakeupGhostTime > 0.0f)
        {
            ForceWakeupGhostTime -= p_delta;
            if (ForceWakeupGhostTime <= 0.0f)
            {
                Events.EmitBlinkyWakeupScoreHit();
                Events.EmitInkyWakeupScoreHit();
                Events.EmitPinkyWakeupScoreHit();
                Events.EmitClydeWakeupScoreHit();
            }
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
        if (score1 > Settings.HighScore)
            Settings.HighScore = score1;
        if (score2 > Settings.HighScore)
            Settings.HighScore = score2;

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

            if (score1 >= CherrySpawnScore && !spawnedCherriesThisRound)
            {
                spawnedCherriesThisRound = true;
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

            if (score2 >= CherrySpawnScore && !spawnedCherriesThisRound)
            {
                spawnedCherriesThisRound = true;
                SpawnCherries();
            }
        }
    }

    private void SpawnCherries()
    {
        RandomNumberGenerator rng = new();

        int index = rng.RandiRange(0, freePowerupPositions.Count - 1);
        Vector2I cell1 = freePowerupPositions[index] * 16 + new Vector2I(8, 8);
        SpawnCherry(cell1);
        freePowerupPositions.Remove(cell1);

        int index2 = rng.RandiRange(0, freePowerupPositions.Count - 1);
        while (index2 == index)
            index2 = rng.RandiRange(0, freePowerupPositions.Count - 1);
        Vector2I cell2 = freePowerupPositions[index2] * 16 + new Vector2I(8, 8);
        SpawnCherry(cell2);
        freePowerupPositions.Remove(cell2);
    }

    private void SpawnCherry(Vector2I cell)
    {
        if (CherryScene == null)
        {
            GD.PushError("Level1.cs: CherryScene is null, cannot spawn cherry!");
            return;
        }

        Node2D cherry = CherryScene.Instantiate<Node2D>();
        if (cherry == null)
        {
            GD.PushError("Level1.cs: Failed to instantiate CherryScene!");
            return;
        }

        cherry.Position = cell; // assuming each cell is 16x16 pixels
        AddChild(cherry);
    }

    private void UpdateLifes(int p_life)
    {
        if (p_life != -1 && p_life != 1)
        {
            GD.PushWarning("Level1.cs: Tried to add/remove more than 1 life at a time!");
            return;
        }

        if (p_life == -1)
        {
            if (lifes < 1)
            {
                GD.PushWarning("Level1.cs: Tried to remove life when having none!");
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
                GD.PushWarning("Level1.cs: Tried to add life when having max");
                return;
            }

            lifes += 1;
        }

        Life1.Visible = lifes >= 1;
        Life2.Visible = lifes >= 2;
        Life3.Visible = lifes >= 3;
    }

    public override void _ExitTree()
    {
        Settings.SaveSettings();
        Events.EmitLeftGameScene();
        Events.ResetLevel1Events();
    }
}
