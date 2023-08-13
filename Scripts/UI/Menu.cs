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

    [Export]
    public PackedScene DecorMenuSlotScene;
    public bool Opened{get; private set;}

    public AnimationPlayer AnimationPlayer{get;private set;}
    private ScrollContainer _scroll_container;
    private VBoxContainer _vbox_container;
    public Position2D Treshold;

    public override void _Ready()
    {
        AnimationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _scroll_container = (ScrollContainer)GetNode("ScrollContainer");
        _vbox_container = (VBoxContainer)_scroll_container.GetNode("VBoxContainer");
        Treshold = (Position2D)GetNode("Treshold");

        Directory dir = new Directory();
        dir.Open("res://Scenes/Decoration/");
        dir.ListDirBegin(true, true);

        string filename = dir.GetNext();

        while(filename != "")
        {
            DecorationMenuSlot newMenuSlot = (DecorationMenuSlot)DecorMenuSlotScene.Instance();
            newMenuSlot.Decoration = (Decoration)GD.Load<PackedScene>("res://Scenes/Decoration/" + filename).Instance();

            _vbox_container.AddChild(newMenuSlot);
            filename = dir.GetNext();
        }
    }

    private void _on_BuildButton_pressed()
    {
        EmitSignal(nameof(BuildButton_pressed));
        if(!Opened)
            AnimationPlayer.Play("Opening");
        else AnimationPlayer.PlayBackwards("Opening");

        Opened = !Opened;
    }

    public void SortDecorations()
    {
        Godot.Collections.Array menuSlots = _vbox_container.GetChildren();

        foreach(object obj in menuSlots)
        {
            DecorationMenuSlot menuSlot = obj as DecorationMenuSlot;

        }
    }

    public void DeleteDecorsMenuSlot(Decoration decoration)
    {
        Godot.Collections.Array allMenuSlots = _vbox_container.GetChildren();
        foreach(object obj in allMenuSlots) 
        {
            if(obj is DecorationMenuSlot dms && dms.Decoration.Filename == decoration.Filename)
            {
                dms.QueueFree();
                return;
            }
        }
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


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
