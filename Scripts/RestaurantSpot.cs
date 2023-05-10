using Godot;
using System;

public partial class RestaurantSpot : Node3D
{

	private CourtArea _parent;
	[Export]
	public PackedScene RestaurantScene;
	[Export]
	public float MealPriceValue, CostValue;
	[Export]
	public string MealPriceMagnitude, CostMagnitude;
	public Tuxdollar MealPrice, Cost;
	[Export]
	public double  WaitTime;

	private PopupMenu _popupMenu;
	private Label _costLabel;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		_parent = (CourtArea)this.GetParent();	
		
		MealPrice = new Tuxdollar(MealPriceValue, MealPriceMagnitude);
		Cost = new Tuxdollar(CostValue, CostMagnitude);

		// Get references to child nodes
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");

		// Set the label text for the Cost label
		_costLabel.Text = $"Cost: {Cost}";
		
		// Connect signals for ConfirmationButton and CancelButton
		//_popupMenu.GetNode<Button>("ConfirmationButton").Connect("pressed", Callable.From(this._on_confirmation_button_pressed));
		//_popupMenu.GetNode<Button>("CancelButton").Connect("pressed", Callable.From(this._on_cancel_button_pressed));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place")) 
			_popupMenu.Popup();
	}

	private void _on_confirmation_button_pressed()
	{
		if(_parent.Parent.Money < Cost) return;
		_parent.Parent.TransferMoney(-Cost);
		RestaurantBase rest = RestaurantScene.Instantiate<RestaurantBase>();
		rest.Position = this.Position + Vector3.Up;
		rest.Rotation = this.Rotation;
		rest.MealPrice = this.MealPrice;
		rest.WaitTime = this.WaitTime;
		rest.Cost = this.Cost;
		this.QueueFree();
		_parent.AddChild(rest);
		//GD.Print(_parent.Restaurants[0]);
		GD.Print("created");
		_parent.GetNode<CustomerSpawner>("CustomerSpawner").Change_wait_time();

		// Hide the PopupMenu
		_popupMenu.Hide();
	}

	private void _on_cancel_button_pressed()
	{
		// Hide the PopupMenu
		_popupMenu.Hide();
	}

	
}

	// public void openRestaurantOptions()
	// {
	// 	if(_myRestaurant != null) return;
	// 	GD.Print(this.Position);
	// 	RestaurantOptions.Text = $"1: 1400$\n2: {130*14}$\n3: {160*14}$\n4: {190*14}$\n5: {220*14}$\n6: {250*14}$\n7: {280*14}$\n8: {310*14}$\n9: {340*14}$\n";
	// 	_menu_Open = true;
	// }


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
