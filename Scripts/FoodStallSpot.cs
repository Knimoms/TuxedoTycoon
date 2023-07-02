using Godot;
using System;

public partial class FoodStallSpot : Spatial
{

	
	[Export]
	public PackedScene RestaurantScene;

	public Tuxdollar Cost = new Tuxdollar();

	[Export]
	public PackedScene FoodStallModel;

	public static BaseScript Parent;
	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;

	private ulong _input_time;

	FoodStall rest;


	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		rest = RestaurantScene.Instance<FoodStall>();
		Cost = new Tuxdollar(rest.Level1CostValue, rest.Level1CostMagnitude);
		if(Parent == null)
			Parent = (BaseScript)GetParent();
		Parent = (BaseScript)this.GetParent();	

		// Get references to child nodes
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		Parent.Spots.Add(this);

		//if(Name == "RestaurantSpot") _instantiate_restaurant();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

	}

	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left) 
			return;

		if(!event1.IsPressed() && Parent.MaxInputDelay.TimeLeft > 0) 
		{
			_popupMenu.PopupCentered();	
			_costLabel.Text = $"Cost: {Cost}";
		}
			
	}

	private void _add_restaurant()
	{
		FoodStall rest = RestaurantScene.Instance<FoodStall>();
		rest.Transform = new Transform(this.Transform.basis, this.Transform.origin + Vector3.Up);
		rest.Rotation = this.Rotation;
		rest.Cost = Cost;
		Parent.Spots.Remove(this);
		this.QueueFree();
		Parent.AddChild(rest);
	}

	private void _on_ConfirmationButton_pressed()
	{
		if(Parent.Money < Cost) 
			return;
		Parent.TransferMoney(-Cost);
		_add_restaurant();
		// Hide the PopupMenu
		_popupMenu.Hide();		
	}

	private void _on_CancelButton_pressed()
	{
		// Hide the PopupMenu
		_popupMenu.Hide();
	}

	 public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motionEvent)
		{
			if (Parent.Money < Cost)
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
