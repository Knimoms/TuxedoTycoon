using Godot;
using System;

public partial class Chair : Node3D
{
	public bool Occupied = false;
	static int i = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetParent().GetParent<CourtArea>().Chairs.Add(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
