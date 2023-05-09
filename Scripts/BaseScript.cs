using Godot;
using System;
using System.Collections.Generic;

public partial class BaseScript : Node
{
	[Export]
	public float StartMoneyValue;
	[Export]
	public string StartMoneyMagnitude;

	public Tuxdollar Money;
	public Label MoneyLabel;
	private RestaurantBase _rest;
	

	public List<RestaurantBase> Restaurants;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Money = new Tuxdollar();
		this.MoneyLabel = (Label)GetNode("MoneyLabel");
		this.Restaurants = GetNode<CustomerSpawner>("CustomerSpawner").Rests;
		TransferMoney(new Tuxdollar(StartMoneyValue, StartMoneyMagnitude));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//if(Input.IsActionJustPressed("place")) 
		//{
		//	GD.Print("created");
		//	Restaurant rest = this.RestaurantScene.Instantiate<Restaurant>();
		//	Restaurants.Add(rest);
		//	rest.MoneyPerSecond = 100*Restaurants.Count;
		//	this.AddChild(rest);
		//}

	}

	private void _on_floor_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		
		//if(event1 is InputEventMouseButton) GetNode<AnimalBase>("AnimalBase").target = postition;
	}

	public void TransferMoney(Tuxdollar Money)
	{
		this.Money += Money;
		MoneyLabel.Text = $"Money: {this.Money}";
	}
}
