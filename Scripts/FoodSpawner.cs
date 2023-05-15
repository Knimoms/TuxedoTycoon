using Godot;
using System;

public partial class FoodSpawner : Node3D
{
	public KitchenCounter kitchenCounter;
	public override void _Ready()
	{
		kitchenCounter = GetParent<KitchenCounter>();
	}
	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place"))
        {
            GD.Print("This is FoodSpawner!");
			GD.Print(kitchenCounter.minigame.CompareLists());
        }
	}
}
