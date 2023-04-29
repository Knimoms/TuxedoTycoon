using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerSpawner : Node3D
{
	[Export]
	public PackedScene CustomerScene;
	private Random _rnd;
	private Timer _timer;
	public List<RestaurantBase> Rests;
	// List<CustomerBase> Customer
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.GetParent()._Ready();
		_timer = (Timer)GetNode("Timer");
		_rnd = new ();
		Rests = new ();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		

	}

	private void _on_timer_timeout()
	{

		GD.Print(GetParent<BaseScript>().Restaurants);
		//_rests = GetParent<BaseScript>().Restaurants;
		if(Rests.Count == 0) return;
		CustomerBase customer = this.CustomerScene.Instantiate<CustomerBase>();
		customer.TargetRestaurant = Rests[_rnd.Next(0,Rests.Count)];
		customer.Position = this.Position;
		this.AddSibling(customer);
	}
}
