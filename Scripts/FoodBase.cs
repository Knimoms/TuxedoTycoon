using Godot;
using System;

public partial class FoodBase : Node3D
{
    FoodSpawner foodSpawner;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foodSpawner = GetParent<FoodSpawner>();
	}

    
}
