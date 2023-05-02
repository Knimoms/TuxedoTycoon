using Godot;
using System;

public partial class RestaurantBase : Node3D
{
	public double MealPrice, WaitTime, Cost, OriginalMealPrice;
	[Export]
	public int CustomerCapacity = 3;
	public CustomerBase CurrentCustomer;

	private static BaseScript _parent = null;

	public int Lvl = 1;
	private Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
		if(_parent == null) _parent = GetParent<BaseScript>();
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
		
		_parent.TransferMoney(MealPrice);
		this.CurrentCustomer.LeaveRestaurant();

	}

	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(Input.IsActionJustPressed("place")) 
			LevelUp();
	}

	public void Order(CustomerBase customer)
	{
		this.CurrentCustomer = customer;
		this._timer.Start();
	}

	public void LevelUp()
	{
		
		double cost  = Math.Pow(this.Cost, 1+Lvl*0.1);
		GD.Print(cost);
		if(_parent.Money < cost) return;
		_parent.TransferMoney(-cost);
		this.MealPrice = Math.Pow(this.OriginalMealPrice, 1+Lvl*0.1);
		Lvl++;
	}

	public Timer TimerProp
	{
		get { return _timer; }
	}
}
