using Godot;

namespace PacMan;

public partial class SettingsMenu : Control
{
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
        }
    }

    private void _on_back_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
    }
}
