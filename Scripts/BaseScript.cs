using Godot;
using System;
using System.Collections.Generic;

public partial class BaseScript : Spatial
{
	[Export]
	public float StartMoneyValue;
	[Export]
	public string StartMoneyMagnitude;

	public Tuxdollar Money = new Tuxdollar(0);
	public Label MoneyLabel;

	public List<Spatial> Spots = new List<Spatial>();
	//private RestaurantBase _rest;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("mooh");
		foreach(Spatial n3d in Spots)
			n3d.Visible = false;
		
		this.MoneyLabel = (Label)GetNode("MoneyLabel");
		TransferMoney(new Tuxdollar(StartMoneyValue, StartMoneyMagnitude));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
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

	public void _on_Button_toggled(bool button_pressed)
	{
		GD.Print("muuh");
		foreach(Spatial n3d in Spots)
			n3d.Visible = button_pressed;
	}
	
	/*
	public class UnlockCourt(){
	//assign each stall a money value, crosscheck player currency with stall value
	
	public double playerCurrency; //player amount value
	public bool isEnough() = false; //check if player has enough money defaul they dont sadly\
	public double restaurantSpotValue;
	
	//assigned player taps restaurant spot, open up restaurant spot value
	if( restaurantSpotValue <= playerCurrency ){
	isEnough == true;
	
	//display stall or whatever
	}
	
	}
	
	*/
}
