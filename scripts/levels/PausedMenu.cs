using Godot;

namespace PacMan;

public partial class PausedMenu : CanvasLayer
{
#pragma warning disable IDE1006 // Naming Styles
    private static void _on_resume_button_pressed()
    {
        Events.EmitUnpaused();
    }

    private void _on_settings_button_pressed()
    {
        // TODO: settings should check if came from menu or game to go back to scene 
        // (if from game, state should be preserved)
        GetTree().ChangeSceneToFile("res://scenes/ui/settings.tscn");
    }

    private void _on_main_menu_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
    }

    private void _on_quit_button_pressed()
    {
        GetTree().Quit();
    }

    private void _on_restart_button_pressed()
    {
        GetTree().ReloadCurrentScene();
    }
#pragma warning restore IDE1006 // Naming Styles
}
