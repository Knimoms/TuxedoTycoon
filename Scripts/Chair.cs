using Godot;
using System;
using System.Collections.Generic;

public partial class Chair : Spatial
{
    public bool Occupied = false;
    public bool unlocked = false;

    public override void _Ready()
    {
        AddToGroup("Persist");
        GetParent().GetParent().GetParent<BaseScript>().Chairs.Add(this);
        Owner = GetParent();
    }

    public override void _Process(float delta)
    {
    }

    public void MakeUsable()
    {
        unlocked = true;
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
            {"RotationY", Rotation.y},
            {"Unlocked", unlocked}
			
		};
	}
}