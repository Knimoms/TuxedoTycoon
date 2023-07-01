using Godot;
using System;
using System.Collections.Generic;

public partial class FoodStall : Spatial
{

	[Export]
	public PackedScene Minigame2DScene;
	public Tuxdollar Multiplicator = new Tuxdollar(1), Cost, TimeUpgradeCost, QualityUpgradeCost;
	public Customer CurrentCustomer;

	public List<Customer> IncomingCustomers = new List<Customer>();
	//public List<CustomerBase> Queue;

	public CourtArea Parent = null;

	private static BaseScript _base_script;

	public Timer TimerProp
	{
		get { return _timer; }
	}
	
	private Timer _timer;
	
	private PopupMenu _popupMenu;
	private Button _foodQualityUpgradeButton;
	private Button _cookingTimeUpgradeButton;
	private Button _cancelButton;
	private Button _levelUpButton;

	private Label _timeCostLabel;
	private Label _qualityCostLabel;
	private Label _nameLabel;
	private Label _levelUpCostLabel;
	private Label _levelLabel;
	
	private Spatial _my_mesh_instance;

	[Export]
	private float CustomersPerMinute = 6;

	public Dish OrderedDish;

	[Export]
	public PackedScene Dish1;
	[Export]	
	public float Level1CostValue;
	[Export]
	public string Level1CostMagnitude;
	public Tuxdollar Level1Cost;

	[Export]
	public PackedScene Dish2;
	[Export]	
	public float Level2CostValue;
	[Export]
	public string Level2CostMagnitude;

	[Export]
	public PackedScene Dish3;
	[Export]	
	public float Level3CostValue;
	[Export]
	public string Level3CostMagnitude;
	
	private Tuxdollar LevelUpCost;
	public List<Dish> Dishes {get; private set;}

	public int Level = 1;

	public Random rnd = new Random();
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		Level1Cost = new Tuxdollar(Level1CostValue, Level1CostMagnitude);
		Dishes = new List<Dish>();

		AddDish(Dish1, Level1Cost);

		LevelUpCost = new Tuxdollar(Level2CostValue, Level2CostMagnitude);
		
		_my_mesh_instance = GetNode<Spatial>("MeshInstance");
		Parent = GetParent<CourtArea>();
		if(_base_script == null)
			_base_script = Parent.Parent;
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = 60/CustomersPerMinute;
		_base_script.Restaurants.Add(this);
		
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		_foodQualityUpgradeButton = _popupMenu.GetNode<Button>("FoodQualityUpgradeButton");
		_foodQualityUpgradeButton.Connect("pressed",this, nameof(_on_FoodQualityUpgradeButton_pressed));

		_cookingTimeUpgradeButton = _popupMenu.GetNode<Button>("CookingTimeUpgradeButton");
		_cookingTimeUpgradeButton.Connect("pressed", this, nameof(_on_CookTimeUpgradeButton_pressed));

		_cancelButton = _popupMenu.GetNode<Button>("CancelButton");
		_cancelButton.Connect("pressed",this, nameof(_on_CancelButton_pressed));

		_levelUpButton = _popupMenu.GetNode<Button>("LevelUpButton");
		_levelUpButton.Connect("pressed", this, nameof(LevelUp));


		_timeCostLabel = _popupMenu.GetNode<Label>("CostLabel");
		_qualityCostLabel = _popupMenu.GetNode<Label>("CostLabel2");
		_nameLabel = _popupMenu.GetNode<Label>("NameLabel");
		_levelUpCostLabel = _popupMenu.GetNode<Label>("CostLabel3");
		_levelLabel = _popupMenu.GetNode<Label>("LevelLabel");

		_levelLabel.Text = $"Lvl {Level}";

		_foodQualityUpgradeButton.Disabled = true;
		_cookingTimeUpgradeButton.Disabled = true;

		TimeUpgradeCost = 4*Cost;
		QualityUpgradeCost = 4*Cost;
		_nameLabel.Text = "Restaurant Name";
		_timeCostLabel.Text = $"{TimeUpgradeCost}";
		_qualityCostLabel.Text = $"{QualityUpgradeCost}";
	}

	public void MiniGameDone()
	{
		int j = IncomingCustomers.Count;
		// for(int i = 0; i < 5 && i < j; i++)
		// {
		// 	if(IncomingCustomers[0].State != CustomerState.WaitingInQueue)
		// 		break;
		// 	IncomingCustomers[0].TakeAwayFood();
		// 	_base_script.TransferMoney(OrderedDish.MealPrice*1.25f);
		// }

		IncomingCustomers[0].TakeAwayFood();
		_base_script.TransferMoney(OrderedDish.MealPrice*Multiplicator*1.9f);
		OrderedDish = null;

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

	private void _on_MiniGame2D_pressed()
	{
		if(TimerProp.TimeLeft == 0)
			return;

		Minigame2D minigame2D = Minigame2DScene.Instance<Minigame2D>();

		ToggleMiniGameMode();
		_base_script.IState = InputState.MiniGameOpened;
		AddChild(minigame2D);

		_popupMenu.Hide();

		_timer.Stop();
	}

	public void CloseMiniGame()
	{
		ToggleMiniGameMode();

		if(IncomingCustomers.Count != 0 && IncomingCustomers[0].State == CustomerState.WaitingInQueue)
			_timer.Start();
		
		_base_script.IState = InputState.Default;
	}

	public void ToggleMiniGameMode()
	{
		_base_script.AverageSatisfactionLabel.Visible = !_base_script.AverageSatisfactionLabel.Visible;
		_base_script.BuildButton.Visible = !_base_script.BuildButton.Visible;
	}

	public void AddDish(PackedScene scene, Tuxdollar mealprice)
	{
		Dish dish = (Dish)scene.Instance();
		dish.MealPrice = mealprice;
		Dishes.Add(dish);
		AddChild(dish);
	}

	public void ToggleVisibility ()
	{
		_my_mesh_instance.Visible = !_my_mesh_instance.Visible;
		Parent.Parent.BuildButton.Visible = !Parent.Parent.BuildButton.Visible;
		Parent.Parent.MoneyLabel.Visible = !Parent.Parent.MoneyLabel.Visible;
	}

	private void _on_Timer_timeout()
	{
		if(IncomingCustomers.Count == 0)
			return;
		this.IncomingCustomers[0].FinishOrder();
		
		for(int i = 0; i < this.IncomingCustomers.Count; i++)
		{
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
		if(!(event1 is InputEventMouseButton) || event1.IsPressed() || _base_script.IState == InputState.MiniGameOpened || _base_script.MaxInputDelay.TimeLeft <= 0)
			return;
			
		if(_base_script.BuildMode)
		{
			ShowPopupMenu();
			return;
		}

		_on_MiniGame2D_pressed();		
	}

	public Dish Order()
	{
		OrderedDish = Dishes[rnd.Next(0,Dishes.Count)];
		this._timer.Start();

		return OrderedDish;
	}

	public void LevelUp()
	{
		if (Level >= 3)
			return;

		Level++;

		_levelLabel.Text = $"Lvl {Level}";
		_base_script.TransferMoney(-LevelUpCost);

		if(Level == 2)
		{
			LevelUpCost = new Tuxdollar(Level3CostValue, Level3CostMagnitude);
			_levelUpButton.Disabled = _base_script.Money < LevelUpCost;
			AddDish(Dish2, Level1Cost*2f);
			return;
		}

		_levelUpButton.Disabled = true;
		AddDish(Dish3, Level1Cost*4f);
	}

	public void FoodQualityLevelUp()
	{
		if(_base_script.Money < QualityUpgradeCost) 
			return;

		_base_script.TransferMoney(-QualityUpgradeCost);
		Multiplicator *= 1.25f;
		QualityUpgradeCost *= 4;
		_qualityCostLabel.Text = $"{QualityUpgradeCost}";
	}

	public void CookingTimeLevelUp()
	{
		if(_base_script.Money < TimeUpgradeCost) 
			return;

		_base_script.TransferMoney(-TimeUpgradeCost);
		TimeUpgradeCost *= 4;
		CustomersPerMinute *= 1.1f;
		_timer.WaitTime = 60/CustomersPerMinute;
		_timeCostLabel.Text = $"{TimeUpgradeCost}";
	}
	
	public void ShowPopupMenu()
	{
		_cookingTimeUpgradeButton.Disabled = _base_script.Money < TimeUpgradeCost;
		_foodQualityUpgradeButton.Disabled = _base_script.Money < QualityUpgradeCost;
		_levelUpButton.Disabled = _base_script.Money < LevelUpCost || Level >= 3;

		// Set the name and cost in the PopupMenu
		
		_popupMenu.PopupCentered();
	}

	public Tuxdollar MoneyPerMinute()
	{
		Tuxdollar averageMealPrice = Tuxdollar.ZeroTux;
		foreach (Dish dish in Dishes)
			averageMealPrice += dish.MealPrice;

		averageMealPrice /= Dishes.Count;

		return averageMealPrice * Multiplicator * Math.Min(CustomersPerMinute, _base_script.Spawner.BonusCustomersPerMinute);		
	}

	public void Refund(Dish dish)
	{
		_base_script.TransferMoney(-dish.MealPrice*Multiplicator);
	}

	private void _on_CookTimeUpgradeButton_pressed()
	{
		CookingTimeLevelUp();
		ShowPopupMenu();
	}

	private void _on_FoodQualityUpgradeButton_pressed()
	{
		FoodQualityLevelUp();
		ShowPopupMenu();
	}

	private void _on_CancelButton_pressed()
	{
		_popupMenu.Hide();
	}

	private void _on_LevelUpButton_pressed()
	{
		
	}
}
