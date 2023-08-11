using Godot;
using System;

public class Menu : AnimatedSprite
{
    [Signal]
    public delegate void BuildButton_pressed();

    [Signal]
    public delegate void RecipeButton_pressed();

    [Signal]
    public delegate void MuteButton_pressed();

    public bool Opened{get; private set;}

    public AnimationPlayer AnimationPlayer{get;private set;}
    private ScrollContainer _scroll_container;

    public override void _Ready()
    {
        AnimationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _scroll_container = (ScrollContainer)GetNode("ScrollContainer");
    }

    private void _on_BuildButton_pressed()
    {
        EmitSignal(nameof(BuildButton_pressed));
        if(!Opened)
            AnimationPlayer.Play("Opening");
        else AnimationPlayer.PlayBackwards("Opening");

        Opened = !Opened;
    }

    private void _on_RecipeButton_pressed()
    {
        EmitSignal(nameof(RecipeButton_pressed));
    }

    private void _on_MuteButton_pressed()
    {
        EmitSignal(nameof(MuteButton_pressed));
    }

    private void ToggleScrollable()
    {
        _scroll_container.Visible = !_scroll_container.Visible;
    }

    public override void _Input(InputEvent @event)
    {
        return;
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
