using Godot;
using System;
using System.Collections.Generic;



public partial class Ingredient : Spatial
{	
	[Export]
	public Texture png;



	[Export]
	public string IngName { get; set; }

	[Export]
	public PackedScene IngredientBaseScene;

	public KitchenCounter kitchenCounter;

	public override void _Ready()
	{
		kitchenCounter = GetParent<KitchenCounter>();
		GetNode("StaticBody").GetNode<Sprite3D>("Sprite3D").Texture = png;
	}

	
	private void _on_StaticBody_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(event1 is InputEventMouseButton && event1.IsPressed())
		{
			AddInUserlist(this);
			foreach(Ingredient a in kitchenCounter.minigame.userList ){
				GD.Print(a.IngName);
			}

			IngredientBase ingredientBase = IngredientBaseScene.Instance<IngredientBase>();
			ingredientBase.Transform = new Transform(ingredientBase.Transform.basis, kitchenCounter.foodSpawner.Transform.origin+Vector3.Up*0.005f);
			GetParent().CallDeferred("add_child", ingredientBase);

			AddInIngredientlist(ingredientBase);
			
		}
	}

	public void AddInUserlist (Ingredient ing)
	{
		kitchenCounter.minigame.userList.Add(ing);
	}

	public void AddInIngredientlist (IngredientBase ingB)
	{
		kitchenCounter.minigame.ingredientList.Add(ingB);
	}

}
