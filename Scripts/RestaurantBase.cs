using Godot;
using System;
using System.Collections.Generic;

public partial class RestaurantBase : Node3D
{
	public Tuxdollar MealPrice, OriginalMealPrice, Cost;
	public double WaitTime;
	[Export]
	public int CustomerCapacity = 3;
	public CustomerBase CurrentCustomer;

	public List<CustomerBase> IncomingCustomers;
	//public List<CustomerBase> Queue;

	private CourtArea _parent = null;

	private static BaseScript _base_script;

	public Timer TimerProp
	{
		get { return _timer; }
	}

	public int Lvl = 1;
	private Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		IncomingCustomers = new();
		_parent = (CourtArea)GetParent();
		_base_script ??= _parent.Parent;
		_parent = GetParent<CourtArea>();
		this.OriginalMealPrice = this.MealPrice;
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = this.WaitTime;
		_parent.Restaurants.Add(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_timer_timeout()
	{
		
		_base_script.TransferMoney(MealPrice);
		this.IncomingCustomers[0].FinishOrder();
		this.IncomingCustomers[0] = null;
		
		for(int i = 0; i < this.IncomingCustomers.Count; i++)
		{
			if(i == this.IncomingCustomers.Count - 1) 
			{
				IncomingCustomers.RemoveAt(i);
				break;
			}
			IncomingCustomers[i] = IncomingCustomers[i+1];
			IncomingCustomers[i].Waiting = false;
			IncomingCustomers[i].LineNumber--;
		}

		if(IncomingCustomers.Count > 0) 
		{
			IncomingCustomers[0].FirstInQueue();
		}

	}

	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(Input.IsActionJustPressed("place")) 
			LevelUp();
	}

	public void Order()
	{
		this._timer.Start();
	}

	public void LevelUp()
	{
		
		Tuxdollar cost  =  this.Cost*4;
		GD.Print(cost);
		if(_base_script.Money < cost) return;
		_base_script.TransferMoney(-cost);
		this.MealPrice *= 4;
		this.Cost *= 4;
		Lvl++;
	}
}
