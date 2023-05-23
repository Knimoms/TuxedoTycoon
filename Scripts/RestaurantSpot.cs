using Godot;
using System;

public partial class RestaurantSpot : Spatial
{

	private CourtArea _parent;
	[Export]
	public PackedScene RestaurantScene;
	[Export]
	public float MealPriceValue, CostValue;
	[Export]
	public string MealPriceMagnitude, CostMagnitude;
	public static Tuxdollar MealPrice = new Tuxdollar(), Cost = new Tuxdollar();
	[Export]
	public float  WaitTime;

	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		_parent = (CourtArea)this.GetParent();	
		
		if(MealPrice == new Tuxdollar())
		{
			MealPrice = new Tuxdollar(MealPriceValue, MealPriceMagnitude);
			Cost = new Tuxdollar(CostValue, CostMagnitude);
		}

		// Get references to child nodes
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		if(_parent.Parent == null) _parent.Parent =  _parent.GetParent<BaseScript>();
		_parent.Parent.Spots.Add(this);

		//if(Name == "RestaurantSpot") _instantiate_restaurant();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

	}

	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place")) 
		{
            _popupMenu.PopupCentered();	
			_costLabel.Text = $"Cost: {Cost}";
		}
	}

	private void _instantiate_restaurant()
	{
		RestaurantBase rest = RestaurantScene.Instance<RestaurantBase>();
		rest.Transform = new Transform(this.Transform.basis, this.Transform.origin + Vector3.Up);
		rest.Rotation = this.Rotation;
		rest.MealPrice = MealPrice;
		rest.WaitTime = WaitTime;
		rest.Cost = Cost;
		_parent.Parent.Spots.Remove(this);
		this.QueueFree();
		_parent.AddChild(rest);
		//GD.Print(_parent.Restaurants[0]);
		_parent.GetNode<CustomerSpawner>("Spawner").Change_wait_time();

	}

	private void _on_ConfirmationButton_pressed()
	{
		if(_parent.Parent.Money < Cost) return;
		_parent.Parent.TransferMoney(-Cost);
		_instantiate_restaurant();
		// Hide the PopupMenu
		_popupMenu.Hide();

		if(Name != "RestaurantSpot")
		{
			Cost *= 8;
			MealPrice = Cost/2;
		}
		
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
