using Godot;
using System;

public partial class Chair : Spatial
{
    public bool Occupied = true;

    public override void _Ready()
    {
        GetParent().GetParent().GetParent<CourtArea>().Chairs.Add(this);
    }

    public override void _Process(float delta)
    {
    }

    public void MakeUsable()
    {
        Occupied = false;
        GetParent().GetParent().GetParent<CourtArea>().Chairs.Remove(this);
    }
}