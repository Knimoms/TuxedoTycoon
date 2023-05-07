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


	public void Change_wait_time() {
		double avgTimeClutter = 0;
		foreach(RestaurantBase r in Rests){
			avgTimeClutter += r.TimerProp.WaitTime;
		}

		_timer.WaitTime = (avgTimeClutter / Rests.Count) / Rests.Count;

	
	}
	private void _on_timer_timeout()
	{
		//_rests = GetParent<BaseScript>().Restaurants;
		if(Rests.Count == 0) return;
		CustomerBase customer = this.CustomerScene.Instantiate<CustomerBase>();
		customer.TargetRestaurant = Rests[_rnd.Next(0,Rests.Count)];
		customer.Position = this.Position;
		this.AddSibling(customer);
	}


}
