using Godot;
using System;
using System.Collections.Generic;

public partial class CourtArea : Node
{
	public BaseScript Parent;
	public List<RestaurantBase> Restaurants;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Parent = (BaseScript)this.GetParent();
		this.Restaurants = GetNode<CustomerSpawner>("CustomerSpawner").Rests;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
	}
}
