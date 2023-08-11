using Godot;
using System;

public class DecorationComponents : Spatial
{

    private Area _area;

    [Signal]
    public delegate void Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx);

    public override void _Ready()
    {
        
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
    {
        EmitSignal(nameof(Area_input_event), camera, event1, postition, normal, shape_idx);
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
