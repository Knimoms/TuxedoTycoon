using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerSpawner : Spatial
{
	[Export]
	public PackedScene CustomerScene;
	private Random _rnd;
	private Timer _timer;
	public List<FoodStall> Rests;

	//private PathFollow3D _path_follow_child;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	 
		_timer = (Timer)GetNode("Timer");
		
		_rnd = new Random();
		Rests = new List<FoodStall>();
	}

	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		

	}


	public void Change_wait_time() {
		float avgTimeClutter = 0;
		foreach(FoodStall r in Rests){
			avgTimeClutter += r.TimerProp.WaitTime;
		}

		_timer.WaitTime = (avgTimeClutter / Rests.Count) / Rests.Count;
		if(Rests.Count == 1)
		{
			_timer.Start();
			_on_Timer_timeout();
		}
	}
	private void _on_Timer_timeout()
	{
		if(Rests.Count == 0) return;
		CustomerBase customer = this.CustomerScene.Instance<CustomerBase>();
		customer.TargetRestaurant = Rests[_rnd.Next(0,Rests.Count)];
		customer.Transform = this.Transform;
		GetParent().AddChild(customer);
		customer.SpawnPoint = GlobalTransform.origin;

	}
}
