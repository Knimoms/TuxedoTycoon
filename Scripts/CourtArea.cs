using Godot;
using System;
using System.Collections.Generic;

public partial class CourtArea : Node
{
	public BaseScript Parent;
	public List<RestaurantBase> Restaurants;
	public List<Table> Tables;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Parent = (BaseScript)this.GetParent();
		// Tables.Add((Table)GetNode("Table"));
		// for(int i = 2; i < 8; i++)
		// {
		// 	Tables.Add((Table)GetNode("Table" + i));
		// }

		// foreach (Table t in Tables)
		// {
		// 	GD.Print(t);
		// }
		this.Restaurants = GetNode<CustomerSpawner>("CustomerSpawner").Rests;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
	}
}
