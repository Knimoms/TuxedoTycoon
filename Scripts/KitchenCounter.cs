using Godot;
using System;

public partial class KitchenCounter : Node
{
	public Minigame minigame;
	public FoodSpawner foodSpawner;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		minigame = GetParent<Minigame>();
		foodSpawner = GetNode<FoodSpawner>("FoodSpawner");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
}
