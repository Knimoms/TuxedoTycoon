using Godot;
using System;
using System.Collections.Generic;


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
    [Export]
    public Texture SoundButtonUnmuted;
    [Export]
    public Texture SoundButtonMuted;
    public bool Opened{get; private set;}

    public AnimationPlayer AnimationPlayer{get;private set;}
    private ScrollContainer _scroll_container;
    private VBoxContainer _vbox_container;
    public Position2D Treshold;
    public Button MuteButton;

    public List<DecorationMenuSlot> decorationMenuSlots = new List<DecorationMenuSlot>();

    public override void _Ready()
    {
        MuteButton = (Button)GetNode("MuteButton");
        AnimationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _scroll_container = (ScrollContainer)GetNode("Cutoff/ScrollContainer");
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
        SortDecorations();
    }

    private void _on_BuildButton_pressed()
    {
        if(BaseScript.DefaultBaseScript.IState == InputState.RecipeBookOpened)
            return;

        EmitSignal(nameof(BuildButton_pressed));
        if(!Opened)
            AnimationPlayer.Play("Opening");
        else AnimationPlayer.PlayBackwards("Opening");

        Opened = !Opened;
    }

    public void SortDecorations()
    {    
        for (int i = 1; i < decorationMenuSlots.Count; i++)
        {
            var key = decorationMenuSlots[i];
            var flag = 0;
            for (int j = i - 1; j >= 0 && flag != 1;)
            {
                if (key.Decoration.Cost < decorationMenuSlots[j].Decoration.Cost)
                {
                    decorationMenuSlots[j + 1] = decorationMenuSlots[j];
                    j--;
                    decorationMenuSlots[j + 1] = key;
                }
                else flag = 1;
            }
        }

        for(int i = 0; i < decorationMenuSlots.Count; i++)
            _vbox_container.MoveChild(decorationMenuSlots[i], i);
    }

    public void HideDecorsMenuSlot(Decoration decoration)
    {
        foreach(DecorationMenuSlot dms in decorationMenuSlots) 
        {
            if(dms.Decoration.Filename == decoration.Filename)
            {
                dms.Visible = false;
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
        BaseScript.DefaultBaseScript.SoundMuted = !BaseScript.DefaultBaseScript.SoundMuted;
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
