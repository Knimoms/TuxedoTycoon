using Godot;
using System;
using System.Collections.Generic;

public partial class Table : Spatial
{
	public override void _Ready()
	{
		GetParent<NavigationMeshInstance>().BakeNavigationMesh(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
}
