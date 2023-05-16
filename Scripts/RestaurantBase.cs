using Godot;
using System;
using System.Collections.Generic;

public partial class RestaurantBase : Node3D
{
	public Tuxdollar MealPrice, OriginalMealPrice, Cost;
	public double WaitTime;
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
	
	private PopupMenu _popupMenu;
	private Button _upgradeButton;
	private Button _cancelButton;
	private Label _costLabel;
	private Label _nameLabel;
	
	private bool _popupMenuOpen = false;
	
	
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
		
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		_upgradeButton = _popupMenu.GetNode<Button>("UpgradeButton");
		_cancelButton = _popupMenu.GetNode<Button>("CancelButton");
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_nameLabel = _popupMenu.GetNode<Label>("NameLabel");

		_upgradeButton.Disabled = true;
		//_upgradeButton.Connect("pressed", this, "_on_upgrade_button_pressed");
		
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
			IncomingCustomers[i].LineNumber--;
		}

		if(IncomingCustomers.Count > 0) 
		{
			IncomingCustomers[0].FirstInQueue();
			IncomingCustomers[0].StartTimer();
		}

	}

	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(Input.IsActionJustPressed("place")) 
			ShowPopupMenu();
			//LevelUp();
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
		_costLabel.Text = $"Cost: {Cost * 4}";
	}
	
	public void ShowPopupMenu()
	{
		_upgradeButton.Disabled = _base_script.Money < Cost * 4;

		// Set the name and cost in the PopupMenu
		_nameLabel.Text = "Restaurant Name";
		_costLabel.Text = $"Cost: {Cost * 4}";
		
		_popupMenu.PopupCentered();

		_popupMenuOpen = true;
	}

	public void Refund()
	{
		_base_script.TransferMoney(-MealPrice);
	}

	private void _on_upgrade_button_pressed()
	{
		if (_popupMenuOpen)
		{
			LevelUp();
			ShowPopupMenu();
		}
	}

	private void _on_cancel_button_pressed()
	{
		if (_popupMenu.Visible)
		{
			_popupMenu.Hide();
			_popupMenuOpen = false;
		}
	}
}
