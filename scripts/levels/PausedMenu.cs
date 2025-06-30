using Godot;

namespace PacMan;

// esc -> settings -> esc -> resume (does not react at all -- prob smth wrong with pause menu)
// esc -> settings -> esc -> esc (need to esc twice -- prob bcz of EmitPaused in SettingsMenu)

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
        GetNode<CanvasLayer>("/root/Level1/PausedMenu").Visible = false;
        GetNode<Node2D>("/root/Level1").Visible = false;

        Globals.CameFromGame = true;

        Node settings = ResourceLoader.Load<PackedScene>("res://scenes/ui/settings.tscn").Instantiate();
        GetTree().Root.AddChild(settings);
    }

    private void _on_main_menu_button_pressed()
    {
        Audio.StopAllAudio();
        GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
    }

    private void _on_quit_button_pressed()
    {
        GetTree().Quit();
    }

    private void _on_restart_button_pressed()
    {
        Audio.StopAllAudio();
        GetTree().ReloadCurrentScene();
    }
#pragma warning restore IDE1006 // Naming Styles
}
