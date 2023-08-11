using Godot;
using System;
using System.Collections.Generic;


public partial class FoodStall : Spatial
{

	[Export]
	public PackedScene Minigame2DScene;
	public Tuxdollar Multiplicator, LevelUpCost;

	public float LevelUpCostValue, MultiplicatorValue = 1;
	public string LevelUpCostMagnitude, MultiplicatorMagnitude = "";

	private Tuxdollar UpgradeCost;
	public List<Dish> Dishes {get; private set;}
	[Export]
	public float CustomersPerMinute = 6;

	public int Level = 1;
	public int Stage = 1;

	private int _current_Upgrade_Treshold;
	public List<Customer> IncomingCustomers = new List<Customer>();
	//public List<CustomerBase> Queue;
	private static BaseScript Parent;

	public Timer TimerProp
	{
		get { return _timer; }
	}
	
	private Timer _timer;

	private PopupMenu _popupMenu;
	private Button _levelUpButton;
	private Button _minigameButton;
	private Button _cancelButton;
	private Button _upgradeButton;

	private Label _levelUpCostLabel;
	private Label _qualityCostLabel;
	private Label _nameLabel;
	private Label _levelLabel;
	
	public Spatial Model{get; private set;}

	private Dish _orderedDish;
	public Dish OrderedDish
	{
		get => _orderedDish;
		set{_orderedDish = value; NewDishIndicator.Visible = _orderedDish != null && !_orderedDish.Unlocked;}
	}

	[Export]
	public PackedScene Dish1;
	[Export]	
	public float Stage1CostValue;
	[Export]
	public string Stage1CostMagnitude;
	public Tuxdollar Stage1Cost;

	public bool Dish1Unlocked = false;

	[Export]
	public PackedScene Dish2;
	[Export]	
	public float Stage2CostValue;
	[Export]
	public string Stage2CostMagnitude;

	public bool Dish2Unlocked = false;

	[Export]
	public PackedScene Dish3;
	[Export]	
	public float Stage3CostValue;
	[Export]
	public string Stage3CostMagnitude;

	private Tuxdollar[] _stage_cost_values;

	public bool Dish3Unlocked = false;

	public Dish[] allDishes = new Dish[3];

	public Random rnd = new Random();

	private bool _minigame_started = false;

	private Sprite3D NewDishIndicator;
	private Sprite3D LevelUpAvailableIndicator;
	private string FolderPath;
	public Spatial OrderWindow;
	public Timer AnimationTimer{get; private set;}
	public Timer LevelUpTimer{get; private set;}
	private const float _levelUpTimerStep = 0.005f;
	private int _levelUpTimerCounter = 0;
	private const float _levelUpTimerDefaultTime = 0.3f;
	private const float _levelUpTimerMinTime = 0.05f;
    
	private Particles _poofParticlesInstance;
	private PathFollow _path_follow;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		string[] splittedFilename = Filename.Split('/');
		splittedFilename[splittedFilename.Length-1] = null;
		FolderPath = string.Join("/", splittedFilename);
        
		this.AddToGroup("Persist");

		LevelUpCost = new Tuxdollar(LevelUpCostValue, LevelUpCostMagnitude);
		Stage1Cost = new Tuxdollar(Stage1CostValue, Stage1CostMagnitude);
		_stage_cost_values = new Tuxdollar[]{new Tuxdollar(Stage2CostValue, Stage2CostMagnitude), new Tuxdollar(Stage3CostValue, Stage3CostMagnitude)};
		Dishes = new List<Dish>();

		_initiate_all_dishes();

		Model = (Spatial)GetNode("Spatial");
		for(int i = 0; i < Stage; i++)
			Dishes.Add(allDishes[i]);
		UpgradeCost = new Tuxdollar(Stage2CostValue, Stage2CostMagnitude);
		
		if(Parent == null)
			Parent = (BaseScript)GetParent();

		Parent.MoneyTransfered += CheckButtonMode;
		Parent.Restaurants.Add(this);
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = 60/CustomersPerMinute;
		
		_popupMenu = GetNode<PopupMenu>("PopupMenu");

		_path_follow = (PathFollow)GetNodeOrNull("Path/PathFollow"); 

		AnimationTimer = _popupMenu.GetNode<Timer>("AnimationTimer");
		AnimationTimer.Connect("timeout", this, nameof(_on_AnimationTimer_timeout));

		LevelUpTimer = _popupMenu.GetNode<Timer>("LevelUpTimer");
		LevelUpTimer.Connect("timeout", this, nameof(_on_LevelUpTimer_timeout));

		_levelUpButton = (Button)_popupMenu.GetNode("LevelUpButton");
		_levelUpButton.Connect("pressed",this, nameof(_on_LevelUpButton_pressed));


		//_levelUpButton.Connect("button_up",this, nameof(_on_LevelUpButton_up));
		_levelUpButton.Connect("button_down",this, nameof(_on_LevelUpButton_down));


		_upgradeButton = (Button)_popupMenu.GetNode("UpgradeButton");
		_upgradeButton.Connect("pressed", this, nameof(_on_UpgradeButton_pressed));

		_cancelButton = (Button)_popupMenu.GetNode("CancelButton");
		_cancelButton.Connect("pressed",this, nameof(_on_CancelButton_pressed));

		_minigameButton = (Button)_popupMenu.GetNode("MinigameButton");
		_minigameButton.Connect("pressed", this, nameof(_on_MiniGame2D_pressed));


		


		_levelUpCostLabel = _popupMenu.GetNode<Label>("CostLabel");
		_nameLabel = _popupMenu.GetNode<Label>("NameLabel");
		
		_levelLabel = _popupMenu.GetNode<Label>("LevelLabel");

		_levelLabel.Text = $"Lvl {Level}";

		LevelUpCost = new Tuxdollar(LevelUpCostValue, LevelUpCostMagnitude);
		Multiplicator = new Tuxdollar(MultiplicatorValue, MultiplicatorMagnitude);

		_nameLabel.Text = "";
		_levelUpCostLabel.Text = $"{LevelUpCost}";

		NewDishIndicator = (Sprite3D)GetNode("Indicators/NewDishIndicator");
		LevelUpAvailableIndicator = (Sprite3D)GetNode("Indicators/LevelUpAvailableIndicator");
		NewDishIndicator.Visible = false;
		LevelUpAvailableIndicator.Visible = false;

		CheckButtonMode();
		_changeModel();
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
			IncomingCustomers[i].LineNumber = i;
		

		if(IncomingCustomers.Count > 0) 
		{
			IncomingCustomers[0].FirstInQueue();
			IncomingCustomers[0].StartTimer();
			if(IncomingCustomers[0].State < CustomerState.WalkingToTable)
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
					allDishes[i] = _initiate_dish(Dish1, Stage1Cost);
					allDishes[i].Unlocked = Dish1Unlocked;
					break;
				case 1:
					allDishes[i] = _initiate_dish(Dish2, Stage1Cost*2f);
					allDishes[i].Unlocked = Dish2Unlocked;
					break;
				case 2: 
					allDishes[i] = _initiate_dish(Dish3, Stage1Cost*4f);
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
	}

	public void ToggleVisibility ()
	{
		Model.Visible = !Model.Visible;
		Parent.MoneyLabel.Visible = !Parent.MoneyLabel.Visible;
	}

	private void _on_Timer_timeout()
	{
		if(IncomingCustomers.Count == 0)
			return;
			
		this.IncomingCustomers[0].FinishOrder();
		
		for(int i = 0; i < this.IncomingCustomers.Count; i++)
			IncomingCustomers[i].LineNumber--;

		if(IncomingCustomers.Count > 0) 
		{
			IncomingCustomers[0].FirstInQueue();
			if(IncomingCustomers[0].State < CustomerState.WalkingToTable)
				IncomingCustomers[0].StartTimer();
		}

	}

	private void _on_AnimationTimer_timeout()
	{
		Parent.ShowUIElements();
	}

	private void _on_LevelUpTimer_timeout()
	{
		LevelUp();
		float NewLevelUpTime = LevelUpTimer.WaitTime - _levelUpTimerCounter*_levelUpTimerStep;
		NewLevelUpTime = Mathf.Clamp(NewLevelUpTime, _levelUpTimerMinTime, _levelUpTimerDefaultTime);
		LevelUpTimer.WaitTime = NewLevelUpTime;
		_levelUpTimerCounter++;
	}

	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(!(event1 is InputEventMouseButton) || event1.IsPressed() || Parent.IState == InputState.MiniGameOpened || Parent.MaxInputDelay.TimeLeft <= 0)
			return;
			
		ShowPopupMenu();		
	}

	public Dish Order()
	{	
		OrderedDish = Dishes[rnd.Next(0,Dishes.Count)];
	
		if(!_minigame_started && OrderedDish.Unlocked)
			this._timer.Start();

		return OrderedDish;
	}

	public Vector3 GetEntryQueueSpot(int LineNumber)
	{
		_path_follow.Offset = 1 + LineNumber*0.5f;
		return _path_follow.GlobalTransform.origin;
	}

	public void Refund(Dish dish)
	{
		Parent.TransferMoney(-dish.MealPrice*Multiplicator);
	}

	public void Upgrade()
	{
		if (Stage >= 3)
			return;

		Stage++;
		Parent.HideUIElements();
		Dishes.Add(allDishes[Stage-1]);
		CheckButtonMode();
		_popupMenu.Hide();
		Parent.IsoCam.ZoomTo(Transform.origin, 6f, 1f, this, nameof(FinishedUpgrade));	
	}

	public void FinishedUpgrade()
	{
		_changeModel();
		Parent.EmitPoof(this);
		AnimationTimer.Start();
	}

	private void _changeModel()
	{
		Vector3 rotation = Model.Rotation;
		Model?.QueueFree();
		Model = (Spatial)GD.Load<PackedScene>($"{FolderPath}Stages/Stage{Stage}.tscn").Instance();
		Model.Rotation = rotation;
		AddChild(Model);
		OrderWindow = (Spatial)Model.GetNode("OrderWindow");
	}

	public void LevelUp()
	{
		if(Parent.Money < LevelUpCost)
			return;

		Tuxdollar cost = LevelUpCost;
		LevelUpCost *= 1.1f;
		Parent.TransferMoney(-cost);
		CustomersPerMinute*= 1.01f;
		Multiplicator *= 1.1f;
		_timer.WaitTime = 60/CustomersPerMinute;
		_levelUpCostLabel.Text = $"{LevelUpCost}";
		Level++;
		_levelLabel.Text = $"Lvl {Level}";
		Parent.CalculateCustomersPerMinute();
	}
	
	public void ShowPopupMenu()
	{
		Parent.IsoCam.ZoomTo(Transform.origin + Vector3.Back, 6f, 0.5f);
		_minigameButton.Disabled = Parent.BuildMode;
		_popupMenu.Popup_();
	}

	public void CheckButtonMode()
	{
		_levelUpButton.Disabled = Parent.Money < LevelUpCost;
		if(_levelUpButton.Disabled)	
			LevelUpTimer.Stop();
		LevelUpAvailableIndicator.Visible = !_levelUpButton.Disabled && Parent.IState != InputState.StartScreen;
		_upgradeButton.Disabled = Stage >= 3 || LevelUpCost/1.1f < _stage_cost_values[Stage-1];
	}

	public Tuxdollar MoneyPerMinute()
	{
		Tuxdollar averageMealPrice = Tuxdollar.ZeroTux;
		foreach (Dish dish in Dishes)
			averageMealPrice += dish.MealPrice;

		averageMealPrice /= Dishes.Count;

		return averageMealPrice * Multiplicator * Math.Min(CustomersPerMinute, Parent.Spawner.CustomersPerMinute);		
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
			{"LevelUpCostValue", LevelUpCost.Value},
			{"LevelUpCostMagnitude", LevelUpCost.Magnitude},
			{"CustomersPerMinute", CustomersPerMinute},
			{"Level", Level},
			{"Stage", Stage},
			{"Dish1Unlocked",allDishes[0].Unlocked},
			{"Dish2Unlocked",allDishes[1].Unlocked},
			{"Dish3Unlocked",allDishes[2].Unlocked},

		};
	}

	private void _on_LevelUpButton_pressed()
	{
		LevelUpTimer.Stop();
		LevelUpTimer.WaitTime = _levelUpTimerDefaultTime;
		_levelUpTimerCounter = 0;
	}

	private void _on_UpgradeButton_pressed()
	{
		Upgrade();
	}
	private void _on_CancelButton_pressed()
	{
		_popupMenu.Hide();
	}

	private void _on_LevelUpButton_down()
	{
		LevelUp();
		LevelUpTimer.Start();
	}


	private void _on_LevelUpButton_up()
	{
		//LevelUpTimer.Stop();
	}
}
