using Godot;
using System;
using System.Collections.Generic;

public partial class Table : Node3D
{
	public List<Chair> Chairs = new();
	public override void _Ready()
	{
		GetParent<CourtArea>().Tables.Add(this);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
