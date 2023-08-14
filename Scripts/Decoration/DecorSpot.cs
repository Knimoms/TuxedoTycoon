using Godot;
using System.Collections.Generic;

public class DecorSpot : Spatial
{
    private static BaseScript _base_script;
    public override void _Ready()
    {
        if(_base_script == null)
            _base_script = (BaseScript)GetViewport().GetNode("Spatial");
        _base_script.DecorSpots.Add(this);
    }

    public void _on_DecorSpot_tree_exiting()
    {
        _base_script.DecorSpots.Remove(this);
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
    {
        if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left)
			return;

		if(!event1.IsPressed() && _base_script.MaxInputDelay.TimeLeft > 0) 
		{
			GetParent().AddChild(_base_script.ActiveDecorationSlot.Decoration);
            _base_script.ActiveDecorationSlot.Decoration.Transform = Transform;
            _base_script.UIContainer.Visible = true;

            _base_script.ActiveDecorationSlot.QueueFree();
            _base_script.ActiveDecorationSlot = null;
            foreach(DecorSpot decorSpot in _base_script.DecorSpots)
                decorSpot.Visible = false;
            foreach(Spatial spot in _base_script.Spots)
                spot.Visible = true;
            QueueFree();
			
		}

    }

    public Dictionary<string, object> Save()
	{
		return new Dictionary<string, object>()
		{
            {"Filename", Filename},
			{"Parent", GetParent().GetPath()},
			{"PositionX", Transform.origin.x},
			{"PositionY", Transform.origin.y},
			{"PositionZ", Transform.origin.z},
            {"RotationY", Rotation.y}
		};
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
