using Godot;
using System;

public class FoodStallUI : PopupMenu
{
    private Control _ui_container;
    private AnimationPlayer _animation_player;
    public override void _Ready()
    {
        _ui_container = (Control)GetNode("UIContainer");
        _animation_player = (AnimationPlayer)GetNode("AnimationPlayer");
    }

    public void ToggleUIContainer()
    {
        _ui_container.Visible = !_ui_container.Visible;
    }

    private void _on_PopupMenu_about_to_show()
    {
        _animation_player.Play("PopUp");
    }

    private void _on_PopupMenu_popup_hide()
    {
        _animation_player.PlayBackwards("PopUp");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
