using System;
using Godot;

namespace PacMan;

public partial class Settings : Node
{
    private static readonly ConfigFile configFile = new();
    private static readonly String ConfigFileName = "user://settings.cfg";

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static int HighScore = 0;
    public static float MasterVolume = -10.0f;
    public static float MusicVolume = -10.0f;
    public static float SoundVolume = 0.0f;
#pragma warning restore CA2211 // Non-constant fields should not be visible

    public static bool LoadSettings()
    {
        Error error = configFile.Load(ConfigFileName);
        if (error != Error.Ok)
        {
            GD.PushError("Failed to read settings file: " + error.ToString());

            if (error == Error.FileNotFound)
            {
                SaveSettings();
            }

            return false;
        }

        HighScore = (int)configFile.GetValue("Scores", "HighScore", 0);

        MasterVolume = (float)configFile.GetValue("Audio", "MasterVolume", -10.0f);
        MusicVolume = (float)configFile.GetValue("Audio", "MusicVolume", -10.0f);
        SoundVolume = (float)configFile.GetValue("Audio", "SoundVolume", 0.0f);

        return true;
    }

    public static bool SaveSettings()
    {
        configFile.SetValue("Scores", "HighScore", HighScore);

        configFile.SetValue("Audio", "MasterVolume", MasterVolume);
        configFile.SetValue("Audio", "MusicVolume", MusicVolume);
        configFile.SetValue("Audio", "SoundVolume", SoundVolume);

        Error error = configFile.Save(ConfigFileName);
        if (error != Error.Ok)
        {
            GD.PushError("Failed to save settings file: " + error.ToString());
            return false;
        }

        return true;
    }
}
