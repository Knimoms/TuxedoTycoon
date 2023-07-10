using Godot;
using System;
using System.Collections.Generic;

public partial class FoodStall : Spatial
{

	[Export]
	public PackedScene Minigame2DScene;
	public Tuxdollar Multiplicator, TimeUpgradeCost, QualityUpgradeCost;

	public float TimeUpgradeCostValue, QualityUpgradeCostValue, MultiplicatorValue = 1;
	public string TimeUpgradeCostMagnitude, QualityUpgradeCostMagnitude, MultiplicatorMagnitude = "";

	private Tuxdollar LevelUpCost;
	public List<Dish> Dishes {get; private set;}
	[Export]
	private float CustomersPerMinute = 6;
	public int Level = 1;

	public Customer CurrentCustomer;

	public List<Customer> IncomingCustomers = new List<Customer>();
	//public List<CustomerBase> Queue;
	private static BaseScript Parent;

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

	public Dish OrderedDish;

	[Export]
	public PackedScene Dish1;
	[Export]	
	public float Level1CostValue;
	[Export]
	public string Level1CostMagnitude;
	public Tuxdollar Level1Cost;

	public bool Dish1Unlocked = false;

	[Export]
	public PackedScene Dish2;
	[Export]	
	public float Level2CostValue;
	[Export]
	public string Level2CostMagnitude;

	public bool Dish2Unlocked = false;

	[Export]
	public PackedScene Dish3;
	[Export]	
	public float Level3CostValue;
	[Export]
	public string Level3CostMagnitude;

	public bool Dish3Unlocked = false;

	public Dish[] allDishes = new Dish[3];

	public Random rnd = new Random();

	private bool _minigame_started = false;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		this.AddToGroup("Persist");
		Level1Cost = new Tuxdollar(Level1CostValue, Level1CostMagnitude);
		Dishes = new List<Dish>();

		_initiate_all_dishes();

		for(int i = 0; i < Level; i++)
			Dishes.Add(allDishes[i]);
		LevelUpCost = new Tuxdollar(Level2CostValue, Level2CostMagnitude);
		
		_my_mesh_instance = GetNode<Spatial>("MeshInstance");
		
		if(Parent == null)
			Parent = (BaseScript)GetParent();

		Parent.MoneyTransfered += CheckButtonMode;
		Parent.Restaurants.Add(this);
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = 60/CustomersPerMinute;
		
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

		TimeUpgradeCost = new Tuxdollar(TimeUpgradeCostValue, TimeUpgradeCostMagnitude);
		QualityUpgradeCost = new Tuxdollar(QualityUpgradeCostValue, QualityUpgradeCostMagnitude);
		Multiplicator = new Tuxdollar(MultiplicatorValue, MultiplicatorMagnitude);

		_nameLabel.Text = "Restaurant Name";
		_timeCostLabel.Text = $"{TimeUpgradeCost}";
		_qualityCostLabel.Text = $"{QualityUpgradeCost}";

		foreach(Dish dish in allDishes)
			GD.Print(dish.Unlocked);

		CheckButtonMode();
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
		Parent.TransferMoney(OrderedDish.MealPrice*Multiplicator*1.9f);
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
		Minigame2D minigame2D = Minigame2DScene.Instance<Minigame2D>();


		_minigame_started = true;
		ToggleMiniGameMode();
		Parent.IState = InputState.MiniGameOpened;
		AddChild(minigame2D);

		_popupMenu.Hide();

		_timer.Stop();
	}

	public void CloseMiniGame()
	{
		ToggleMiniGameMode();
		_minigame_started = false;

		if(IncomingCustomers.Count != 0 && IncomingCustomers[0].State == CustomerState.WaitingInQueue)
			_timer.Start();
		
		Parent.IState = InputState.Default;
	}

	private void _initiate_all_dishes()
	{
		for(int i = 0; i < allDishes.Length; i++)
		{
			switch(i)
			{
				case 0:
					allDishes[i] = _initiate_dish(Dish1, Level1Cost);
					allDishes[i].Unlocked = Dish1Unlocked;
					break;
				case 1:
					allDishes[i] = _initiate_dish(Dish2, Level1Cost*2f);
					allDishes[i].Unlocked = Dish2Unlocked;
					break;
				case 2: 
					allDishes[i] = _initiate_dish(Dish3, Level1Cost*4f);
					allDishes[i].Unlocked = Dish3Unlocked;
					break;
				default:
					break;
			}
		}
	}
	private Dish _initiate_dish(PackedScene dishScene, Tuxdollar MealPrice)
	{
		Dish dish = (Dish)dishScene.Instance();
		dish.MealPrice = MealPrice;
		AddChild(dish);
		return dish;
	}

	public void ToggleMiniGameMode()
	{
		Parent.AverageSatisfactionLabel.Visible = !Parent.AverageSatisfactionLabel.Visible;
		Parent.BuildButton.Visible = !Parent.BuildButton.Visible;
		Parent.RecipeButton.Visible = !Parent.RecipeButton.Visible;
	}

	public void ToggleVisibility ()
	{
		_my_mesh_instance.Visible = !_my_mesh_instance.Visible;
		Parent.BuildButton.Visible = !Parent.BuildButton.Visible;
		Parent.MoneyLabel.Visible = !Parent.MoneyLabel.Visible;
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
		if(!(event1 is InputEventMouseButton) || event1.IsPressed() || Parent.IState == InputState.MiniGameOpened || Parent.MaxInputDelay.TimeLeft <= 0)
			return;
			
		if(Parent.BuildMode)
		{
			ShowPopupMenu();
			return;
		}

		_on_MiniGame2D_pressed();		
	}

	public Dish Order()
	{
		OrderedDish = Dishes[rnd.Next(0,Dishes.Count)];
		if(!_minigame_started)
			this._timer.Start();

		return OrderedDish;
	}

	public void LevelUp()
	{
		if (Level >= 3)
			return;

		Level++;

		_levelLabel.Text = $"Lvl {Level}";
		Parent.TransferMoney(-LevelUpCost);

		if(Level == 2)
		{
			LevelUpCost = new Tuxdollar(Level3CostValue, Level3CostMagnitude);
			Dishes.Add(allDishes[1]);
			return;
		}

		_levelUpButton.Disabled = true;
		Dishes.Add(allDishes[2]);
	}

	public void FoodQualityLevelUp()
	{
		if(Parent.Money < QualityUpgradeCost) 
			return;

		Tuxdollar cost = QualityUpgradeCost;
		QualityUpgradeCost *= 4;
		Parent.TransferMoney(-cost);
		Multiplicator *= 1.25f;
		_qualityCostLabel.Text = $"{QualityUpgradeCost}";
	}

	public void CookingTimeLevelUp()
	{
		if(Parent.Money < TimeUpgradeCost) 
			return;

		Tuxdollar cost = TimeUpgradeCost;
		TimeUpgradeCost *= 4;
		Parent.TransferMoney(-TimeUpgradeCost);
		CustomersPerMinute *= 1.1f;
		_timer.WaitTime = 60/CustomersPerMinute;
		_timeCostLabel.Text = $"{TimeUpgradeCost}";
	}
	
	public void ShowPopupMenu()
	{
		_popupMenu.PopupCentered();
	}

	public void CheckButtonMode()
	{
		_cookingTimeUpgradeButton.Disabled = Parent.Money < TimeUpgradeCost;
		_foodQualityUpgradeButton.Disabled = Parent.Money < QualityUpgradeCost;
		_levelUpButton.Disabled = Parent.Money < LevelUpCost || Level >= 3;
	}

	public Tuxdollar MoneyPerMinute()
	{
		Tuxdollar averageMealPrice = Tuxdollar.ZeroTux;
		foreach (Dish dish in Dishes)
			averageMealPrice += dish.MealPrice;

		averageMealPrice /= Dishes.Count;

		return averageMealPrice * Multiplicator * Math.Min(CustomersPerMinute, Parent.Spawner.BonusCustomersPerMinute);		
	}

	public Dictionary<string, object> Save()
	{
		return new Dictionary<string, object>()
		{
			{"Filename", Filename},
			{"Parent", Parent.GetPath()},
			{"PositionX", Transform.origin.x},
			{"PositionY", Transform.origin.y},
			{"PositionZ", Transform.origin.z},
			{"RotationY", Rotation.y},
			{"MultiplicatorValue", Multiplicator.Value},
			{"MultiplicatorMagnitude", Multiplicator.Magnitude},
			{"TimeUpgradeCostValue", TimeUpgradeCost.Value},
			{"TimeUpgradeCostMagnitude", TimeUpgradeCost.Magnitude},
			{"QualityUpgradeCostValue", QualityUpgradeCost.Value},
			{"QualityUpgradeCostMagnitude", QualityUpgradeCost.Magnitude},
			{"CustomersPerMinute", CustomersPerMinute},
			{"Level", Level},
			{"Dish1Unlocked",allDishes[0].Unlocked},
			{"Dish2Unlocked",allDishes[1].Unlocked},
			{"Dish3Unlocked",allDishes[2].Unlocked},

		};
	}

	public void Refund(Dish dish)
	{
		Parent.TransferMoney(-dish.MealPrice*Multiplicator);
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
