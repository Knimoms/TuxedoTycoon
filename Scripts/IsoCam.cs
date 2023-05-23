using Godot;
using System;

public class IsoCam : Spatial
{
    private BaseScript _parent;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _parent = (BaseScript)GetParent();
        
    }

    public override void _Input(InputEvent @event)
	{
		if (!(@event is InputEventMouseMotion motionEvent))
            return;
		
        if(Input.IsActionPressed("place") && !_parent.UIopened)
        {
            this.Translate(new Vector3(motionEvent.Relative.x * -0.05f, 0, motionEvent.Relative.y * -0.05f));

        }		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
