using Godot;
using System;
using System.Collections.Generic;

public partial class FoodStall : Spatial
{

	[Export]
	public PackedScene MiniGameScene;
	public Tuxdollar MealPrice, OriginalMealPrice, Cost;
	public float WaitTime;
	public Customer CurrentCustomer;

	public List<Customer> IncomingCustomers = new List<Customer>();
	//public List<CustomerBase> Queue;

	public CourtArea _parent = null;

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
	public Button MiniGameButton;
	
	private bool _popupMenuOpen = false;

	private MeshInstance _my_mesh_instance;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		_my_mesh_instance = GetNode<MeshInstance>("MeshInstance");
		_parent = GetParent<CourtArea>();
		if(_base_script == null)_base_script = _parent.Parent;
		this.OriginalMealPrice = this.MealPrice;
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = this.WaitTime;
		_parent.Restaurants.Add(this);
		
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		_upgradeButton = _popupMenu.GetNode<Button>("UpgradeButton");
		_cancelButton = _popupMenu.GetNode<Button>("CancelButton");
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_nameLabel = _popupMenu.GetNode<Label>("NameLabel");
		MiniGameButton = _popupMenu.GetNode<Button>("MiniGame");

		_upgradeButton.Disabled = true;
		//_upgradeButton.Connect("pressed", this, "_on_upgrade_button_pressed");
		
	}

	public void MiniGameDone()
	{
		int j = IncomingCustomers.Count;
		for(int i = 0; i < 5 && i < j; i++)
		{
			if(!IncomingCustomers[0].Waiting)
				break;
			IncomingCustomers[0].TakeAwayFood();
			_base_script.TransferMoney(MealPrice*1.25f);
		}

		for(int i = 0; i < IncomingCustomers.Count; i++)
		{
			IncomingCustomers[i].LineNumber = i;
		}

		if(IncomingCustomers.Count > 0) 
		{
			IncomingCustomers[0].FirstInQueue();
			IncomingCustomers[0].StartTimer();
		}

		_timer.Stop();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		 
	}

	private void _on_MiniGame_pressed()
	{
		Minigame minigame = MiniGameScene.Instance<Minigame>();

		_base_script.MiniGameStarted = true;

		AddChild(minigame);
		minigame.Transform = GetNode<Spatial>("MiniGameSpot").Transform;
		minigame.MyFoodStall = this;
		minigame.GetNode<Camera>("Camera").MakeCurrent();

		_popupMenu.Hide();

		_timer.Stop();
		_my_mesh_instance.Visible = false;
		_parent.Parent.BuildButton.Visible = false;
		_parent.Parent.MoneyLabel.Visible = false;

	}

	public void ToggleVisibility ()
	{
		_my_mesh_instance.Visible = !_my_mesh_instance.Visible;
		_parent.Parent.BuildButton.Visible = !_parent.Parent.BuildButton.Visible;
		_parent.Parent.MoneyLabel.Visible = !_parent.Parent.MoneyLabel.Visible;
	}

	private void _on_Timer_timeout()
	{
		if(IncomingCustomers.Count == 0)
			return;
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

	private void _on_StaticBody_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(event1 is InputEventMouseButton && event1.IsPressed() && !_base_script.BuildMode && !_base_script.MiniGameStarted) 
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

	private void _on_UpgradeButton_pressed()
	{
		if (_popupMenuOpen)
		{
			LevelUp();
			ShowPopupMenu();
		}
	}

	private void _on_CancelButton_pressed()
	{
		if (_popupMenu.Visible)
		{
			_popupMenu.Hide();
			_popupMenuOpen = false;
		}
	}
}
