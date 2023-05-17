using Godot;
using System;

public partial class FoodSpawner : Node3D
{
	[Export]
	public PackedScene FoodBaseScene;

	public KitchenCounter kitchenCounter;
	//public PackedScene FoodSpawnerScene;
	public override void _Ready()
	{
		kitchenCounter = GetParent<KitchenCounter>();
		// string path = "res://Scenes/Minigame/FoodBase.tscn";
		// PackedScene packedScene = GD.Load<PackedScene>(path);
		// FoodBase foodBase = packedScene.Instantiate<FoodBase>();
		// foodBase.Position = Position + Vector3.Up;
		// CallDeferred("add_child", foodBase);
	}
	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place"))
		{
			spawnFood();
			GD.Print("This is FoodSpawner!");
			GD.Print(kitchenCounter.minigame.CompareLists());
		}
	}

    public void spawnFood()
    {
        if (kitchenCounter.minigame.CompareLists() == true)
        {
            string path = "res://Scenes/Minigame/FoodBase.tscn";
            PackedScene packedScene = GD.Load<PackedScene>(path);
            FoodBase foodBase = packedScene.Instantiate<FoodBase>();
            foodBase.Position = Position + Vector3.Up;
            CallDeferred("add_child", foodBase);
            GD.Print("NOT POOP!");

        }
        else
        {
            GD.Print("POOP!");
        }
    }
}
