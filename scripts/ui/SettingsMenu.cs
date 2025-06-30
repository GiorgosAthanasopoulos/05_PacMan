using Godot;

namespace PacMan;

public partial class SettingsMenu : Control
{
    [Export]
    public HSlider masterVolumeSlider, musicVolumeSlider, soundVolumeSlider;

    public override void _Ready()
    {
        masterVolumeSlider = GetNode<HSlider>("VBoxContainer/MasterVolumeSlider");
        musicVolumeSlider = GetNode<HSlider>("VBoxContainer/MusicVolumeSlider");
        soundVolumeSlider = GetNode<HSlider>("VBoxContainer/SoundVolumeSlider");

        masterVolumeSlider.SetValueNoSignal(Settings.MasterVolume);
        musicVolumeSlider.SetValueNoSignal(Settings.MusicVolume);
        soundVolumeSlider.SetValueNoSignal(Settings.SoundVolume);
    }

    public override void _Process(double p_delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
            GoBack();
    }

    private void GoBack()
    {
        if (Globals.CameFromGame)
        {
            GetNode<Node2D>("/root/Level1").Visible = true;
            GetNode<CanvasLayer>("/root/Level1/PausedMenu").Visible = true;
            Events.EmitPaused();
            Globals.CameFromGame = false;
            QueueFree();
        }
        else
            GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
    }

#pragma warning disable IDE1006 // Naming Styles
    private void _on_back_button_pressed()
    {
        GoBack();
    }

    private static void _on_master_volume_slider_value_changed(float value)
    {
        Audio.SetMasterVolume(value);
    }

    private static void _on_music_volume_slider_value_changed(float value)
    {
        Audio.SetMusicVolume(value);
    }

    private static void _on_sound_volume_slider_value_changed(float value)
    {
        Audio.SetSoundVolume(value);
    }
#pragma warning restore IDE1006 // Naming Styles
}
