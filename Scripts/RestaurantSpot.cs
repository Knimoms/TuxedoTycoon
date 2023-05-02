using Godot;
using System;

public partial class RestaurantSpot : Node3D
{

	public static BaseScript Parent;
	[Export]
	public PackedScene RestaurantScene;
	[Export]
	public double MealPrice, Cost, WaitTime;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		if(Parent == null)
			Parent = (BaseScript)this.GetParent();	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place")) 
			_createRestaurant();
	}

	// public void openRestaurantOptions()
	// {
	// 	if(_myRestaurant != null) return;
	// 	GD.Print(this.Position);
	// 	RestaurantOptions.Text = $"1: 1400$\n2: {130*14}$\n3: {160*14}$\n4: {190*14}$\n5: {220*14}$\n6: {250*14}$\n7: {280*14}$\n8: {310*14}$\n9: {340*14}$\n";
	// 	_menu_Open = true;
	// }

	public void openRestaurant()
	{

	}

	

	// public void closeRestaurantOptions()
	// {
	// 	RestaurantOptions.Text = $"";
	// 	_menu_Open = false;
	// }

	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	string[] number = {"1","2","3","4","5","6","7","8","9"};
	// 	if(!Array.Exists(number, element => element == @event.AsText()) || !_menu_Open) return;
	// 	closeRestaurantOptions();

	// 	int Value = -1;
	// 	switch(@event.AsText())
	// 	{
	// 		case "1": 
	// 			Value = 100;
	// 			break;

	// 		case "2": 
	// 			Value = 130;
	// 			break;
			
	// 		case "3": 
	// 			Value = 160;			
	// 			break;
			
	// 		case "4": 
	// 			Value = 190;			
	// 			break;

	// 		case "5": 
	// 			Value = 220;
	// 			break;

	// 		case "6": 
	// 			Value = 250;
	// 			break;
				
	// 		case "7": 
	// 			Value = 280;
	// 			break;

	// 		case "8": 
	// 			Value = 310;
	// 			break;

	// 		case "9": 
	// 			Value = 340;
	// 			break;

	// 	}

	// 	if(Parent.Money<Value*14 || Value < 0 ) return;
	// 	GD.Print(this.Name);
	// 	_createRestaurant(Value, Value*14);


	// }

	private void _createRestaurant()
	{
		if(Parent.Money < Cost) return;
		Parent.TransferMoney(-Cost);
		RestaurantBase rest = RestaurantScene.Instantiate<RestaurantBase>();
		rest.Position = this.Position;
		rest.MealPrice = this.MealPrice;
		rest.WaitTime = this.WaitTime;
		rest.Cost = this.Cost;
		this.QueueFree();
		Parent.AddChild(rest);
		GD.Print("created");
		Parent.GetNode<CustomerSpawner>("CustomerSpawner").Change_wait_time();
	}

	


}
