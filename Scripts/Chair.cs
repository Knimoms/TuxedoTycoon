using Godot;
using System;

public partial class Chair : Node3D
{
	public bool Occupied = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(GetParent().GetParent<CourtArea>()+ "hallo");
		GetParent().GetParent<CourtArea>().Chairs.Add(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
