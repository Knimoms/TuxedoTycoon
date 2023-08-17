using Godot;
using System.Collections.Generic;

public class Decoration : Spatial
{
    [Export]
    public float CostValue;
    [Export]
    public string CostMagnitude;

    [Export]
    public int SatisfactionBonus;
    [Export]
    public Texture Preview;

    public Tuxdollar Cost;

    private BaseScript _base_script;

    

    public override void _Ready()
    {
        AddToGroup("Persist");
        Cost = new Tuxdollar(CostValue, CostMagnitude);
        if(_base_script == null)
            _base_script = (BaseScript)GetViewport().GetNode("Spatial");

        _base_script.SatisfactionBonus += SatisfactionBonus;

        Connect("tree_exiting", this, nameof(_on_Decoration_tree_exiting));
        
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(!(event1 is InputEventMouseButton) || event1.IsPressed() || _base_script.IState == InputState.MiniGameOpened || _base_script.MaxInputDelay.TimeLeft <= 0)
			return;
					
	}


    private void _on_Decoration_tree_exiting()
    {
        _base_script.SatisfactionBonus -= SatisfactionBonus;
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
}
