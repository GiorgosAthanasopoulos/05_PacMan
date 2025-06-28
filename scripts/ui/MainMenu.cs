using Godot;

namespace PacMan;

public partial class MainMenu : Control
{
#pragma warning disable IDE1006 // Naming Styles
    private void _on_play_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/levels/level1.tscn");
    }

    private void _on_settings_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/settings.tscn");
    }

    private void _on_quit_button_pressed()
    {
        GetTree().Quit();
    }
#pragma warning restore IDE1006 // Naming Styles
}
