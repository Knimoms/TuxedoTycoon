using Godot;
using System;

public partial class FoodSpawner : Spatial
{
	[Export]
	public PackedScene FoodBaseScene;

	public KitchenCounter kitchenCounter;
	public override void _Ready()
	{
		kitchenCounter = GetParent<KitchenCounter>();
	}
	private void _on_StaticBody_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if (event1 is InputEventMouseButton && event1.IsPressed())
		{
			if(kitchenCounter.minigame.Done)
				return;
			GD.Print(event1);
			foreach (IngredientBase arsch in kitchenCounter.minigame.ingredientList)
			{
				arsch.QueueFree();
			}
			spawnFood();
			kitchenCounter.minigame.userList.Clear();
			kitchenCounter.minigame.ingredientList.Clear();
		}
	}

	public void spawnFood()
	{
		if (kitchenCounter.minigame.CompareLists())
		{
			kitchenCounter.minigame.Done = true;
			FoodBase foodBase = FoodBaseScene.Instance<FoodBase>();
			foodBase.Transform = new Transform(this.Transform.basis, this.Transform.origin + Vector3.Up*0.1f);
			CallDeferred("add_child", foodBase);
			kitchenCounter.minigame.MyFoodStall.MiniGameDone();
		}
		else
		{
			GD.Print("POOP!");
		}
	}
}
