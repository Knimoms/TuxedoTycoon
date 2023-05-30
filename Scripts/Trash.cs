using Godot;
using System;

public class Trash : Button
{
	public Minigame minigame;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		minigame = GetParent<Minigame>();
	}

	private void _on_Button_pressed()
	{
		foreach(IngredientBase arsch in minigame.ingredientList)
		{
			arsch.QueueFree();
		}
		minigame.userList.Clear();
		minigame.ingredientList.Clear();
	}
}
