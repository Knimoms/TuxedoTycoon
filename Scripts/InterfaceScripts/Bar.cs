using Godot;
using System;

public class Bar : HBoxContainer
{
    TextureProgress textprog;
    // Called when the node enters the scene tree for the first time.
    

    public override void _Ready()
    {
        textprog = (TextureProgress)GetNode("TextureProgress");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    private void _on_Timer_timeout()
    {
        textprog.Value +=1;
    }
}
