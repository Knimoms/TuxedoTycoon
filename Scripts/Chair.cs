using Godot;
using System;

public partial class Chair : Spatial
{
	public bool Occupied = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetParent().GetParent().GetParent<CourtArea>().Chairs.Add(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
}
