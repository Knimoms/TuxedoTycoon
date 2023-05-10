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
	private Button _confirmationButton;

	
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
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		// Set the label text for the Cost label
		_costLabel.Text = $"Cost: {Cost}";
		
		// Connect signals for ConfirmationButton and CancelButton
		//_popupMenu.GetNode<Button>("ConfirmationButton").Connect("pressed", Callable.From(this._on_confirmation_button_pressed));
		//_popupMenu.GetNode<Button>("CancelButton").Connect("pressed", Callable.From(this._on_cancel_button_pressed));
		_confirmationButton.Connect("pressed", Callable.From(this._on_confirmation_button_pressed));
		_popupMenu.GetNode<Button>("CancelButton").Connect("pressed", Callable.From(this._on_cancel_button_pressed));
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

	 public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motionEvent)
		{
			if (_parent.Parent.Money < Cost)
			{
				_confirmationButton.Disabled = true;
			}
			else
			{
				_confirmationButton.Disabled = false;
			}
		}
	}
	
}
