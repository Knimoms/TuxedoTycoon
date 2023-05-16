using Godot;
using System;
using System.Collections.Generic;

public partial class CourtArea : Node3D
{
	public BaseScript Parent;
	public List<RestaurantBase> Restaurants;
	public List<Chair> Chairs = new();

	Random rnd = new();
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		Parent = (BaseScript)this.GetParent();
		Restaurants = GetNode<CustomerSpawner>("CustomerSpawner").Rests;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
	}

	public Chair GetRandomFreeChair()
	{
		if(Chairs.Count == 0) return null;
		int i = rnd.Next(0, Chairs.Count), j = i;
		Chair chair = Chairs[i];
		while(chair.Occupied)
		{
			if(i >= Chairs.Count-1) 
				i = -1;
			chair = Chairs[++i];
			if(j == i) return null;
		}

		return chair;
	}
}
