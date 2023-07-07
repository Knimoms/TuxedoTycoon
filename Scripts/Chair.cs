using Godot;
using System;

public partial class Chair : Spatial
{
    public bool Occupied = true;
    public bool unlocked = false;

    public override void _Ready()
    {
        GetParent().GetParent().GetParent<BaseScript>().Chairs.Add(this);
        Owner = GetParent();
    }

    public override void _Process(float delta)
    {
    }

    public void MakeUsable()
    {
        Occupied = false;
        unlocked = true;
    }
}