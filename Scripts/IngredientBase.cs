using Godot;
using System;

public class IngredientBase : StaticBody
{
    
	// Called every frame. 'delta' is the elapsed time since the previous frame.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	public override void _Process(float delta)
	{
		RotateObjectLocal(new Vector3(0, 1, 0), 0.001f);
	}
}
