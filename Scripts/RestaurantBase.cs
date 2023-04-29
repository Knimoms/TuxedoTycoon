using Godot;
using System;

public partial class RestaurantBase : Node3D
{
	public int MoneyPerSecond;
	private static BaseScript _parent = null;

	public int Lvl = 1;
	private Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
		if(_parent == null) _parent = GetParent<BaseScript>();
		_timer = GetNode<Timer>("Timer");
		_parent.Restaurants.Add(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_timer_timeout()
	{
		GD.Print(_timer.WaitTime);
		_parent.TransferMoney(MoneyPerSecond*(int)_timer.WaitTime);

	}
}
