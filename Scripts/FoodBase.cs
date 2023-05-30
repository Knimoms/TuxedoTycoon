using Godot;
using System;

public partial class FoodBase : Spatial
{
    FoodSpawner foodSpawner;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foodSpawner = GetParent<FoodSpawner>();
	}
	public override void _Process(float delta)
	{
		RotateObjectLocal(new Vector3(0, 1, 0), 0.05f);
	}
    
}
