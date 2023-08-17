using Godot;
using System;

public class DecorationComponents : Spatial
{

    private Area _area;
    private BaseScript _base_script;
    private Sprite3D _sprite3D;

    [Signal]
    public delegate void Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx);

    public override void _Ready()
    {
        _base_script = BaseScript.DefaultBaseScript;
        _sprite3D = (Sprite3D)GetNode("Sprite3D");
        _sprite3D.GlobalScale(Vector3.One);
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
    {
        EmitSignal(nameof(Area_input_event), camera, event1, postition, normal, shape_idx);

        if(!(event1 is InputEventMouseButton) || event1.IsPressed() || (int)_base_script.IState >= 5 || _base_script.MaxInputDelay.TimeLeft <= 0 || !_base_script.BuildMode)
			return;

		if(!event1.IsPressed() && _base_script.MaxInputDelay.TimeLeft > 0) 
			_base_script.IsoCam.ZoomTo(this.GlobalTransform.origin, 6f, 0.5f);

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
