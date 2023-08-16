using Godot;
using System;

public class DecorationComponents : Spatial
{

    private Area _area;
    private BaseScript _base_script;

    [Signal]
    public delegate void Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx);

    public override void _Ready()
    {
        _base_script = BaseScript.DefaultBaseScript;
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
    {
        EmitSignal(nameof(Area_input_event), camera, event1, postition, normal, shape_idx);

        if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left) 
			return;

		if(!event1.IsPressed() && _base_script.MaxInputDelay.TimeLeft > 0) 
		{
			_base_script.IsoCam.ZoomTo(this.Transform.origin + Vector3.Back, 6f, 0.5f);
		}
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
