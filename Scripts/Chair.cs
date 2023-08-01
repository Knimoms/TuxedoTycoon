using Godot;
using System;
using System.Collections.Generic;

public partial class Chair : Spatial
{
    public bool Occupied = false;
    public bool unlocked = false;

    public override void _Ready()
    {
        GetParent().GetParent().GetParent<BaseScript>().Chairs.Add(this);
    }

    public override void _Process(float delta)
    {
    }

    public void MakeUsable()
    {
        unlocked = true;
    }
}