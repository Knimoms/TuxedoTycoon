using Godot;
using System;
using System.Collections.Generic;

public partial class BaseScript : Spatial
{
	[Export]
	public float StartMoneyValue;
	[Export]
	public string StartMoneyMagnitude;

	public bool BuildMode = false;

	public int CustomerSatisfactionTotal {get; private set;}
	private Queue<int> _customer_satisfactions = new Queue<int>();

	[Export]
	public int BadRatingMax = 34;

	[Export]
	public int GoodRatingMin = 67;

	private float _satisfactionRating;
	public float SatisfactionRating {
		get => _satisfactionRating;
		set{_satisfactionRating = (_customer_satisfactions.Count != 0)? CustomerSatisfactionTotal/_customer_satisfactions.Count : (BadRatingMax+GoodRatingMin)/2;}
	}

	public List<Chair> Chairs = new List<Chair>();
	public Vector3 SpawnPoint {get; private set;}

	private float _customers_per_minute;
	public float CustomersPerMinute
	{
		get 
		{
			if(_customers_per_minute == 0)
				_customers_per_minute = CalculateCustomersPerMinute();
			return _customers_per_minute;
		}
		set{ _customers_per_minute = CalculateCustomersPerMinute();}
	}


	public InputState IState;
	private Tuxdollar _money;
	public Tuxdollar Money
	{
		get => _money;
		set{_money = value; MoneyTransfered.Invoke();}
	}

	public Label MoneyLabel;
	public List<Spatial> Spots = new List<Spatial>();
	public Timer MaxInputDelay;
	public Vector2 InputPosition;
	public Button BuildButton;
	public Camera BaseCam;
	public List<FoodStall> Restaurants = new List<FoodStall>();
	public CustomerSpawner Spawner;
	public Label AverageSatisfactionLabel;
	public Button RecipeButton;
	public Label CPMLabel;
	public RecipeBook TheRecipeBook;
	public Random rnd = new Random();

	public delegate void CheckButtonModes();

	public event CheckButtonModes MoneyTransfered;

	private double _offline_seconds;
	private double UnixTimestamp;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Money = Tuxdollar.ZeroTux;
		LoadGame();

		RecipeButton = (Button)GetNode("RecipeButton");
		SpawnPoint = GetNode<Spatial>("SpawnPoint").Transform.origin;
		MaxInputDelay = (Timer)GetNode("MaxInputDelay");
		MoneyLabel = (Label)GetNode("MoneyLabel");
		BuildButton = (Button)GetNode("Button");
		BaseCam = (Camera)GetNode("pivot").GetNode("Camera");
		CPMLabel = (Label)GetNode("CPMLabel");
		AverageSatisfactionLabel = (Label)GetNode("AverageSatisfaction");

		if(CustomerSatisfactionTotal != 0)
			AverageSatisfactionLabel.Text = $"Rating: {SatisfactionRating}";
		TheRecipeBook = (RecipeBook)GetNode("RecipeBook");
		TheRecipeBook.FoodStalls = Restaurants;

		TransferMoney(new Tuxdollar(StartMoneyValue, StartMoneyMagnitude));
		CalculateCustomersPerMinute();
		foreach(Spatial spot in Spots)
			spot.Scale = new Vector3(spot.Scale.x , 1, spot.Scale.z);
		
		GD.Print(CalculateMoneyPerMinute());
	}

	public Chair GetRandomFreeChair()
	{
		if(Chairs.Count == 0) return null;
		int i = rnd.Next(0, Chairs.Count), j = i;
		Chair chair = Chairs[i];
		while(chair.Occupied || !chair.unlocked)
		{
			if(i >= Chairs.Count-1) 
				i = -1;
			chair = Chairs[++i];
			if(j == i) return null;
		}

		return chair;
	}

	public void TransferMoney(Tuxdollar Money)
	{
		this.Money += Money;
		MoneyLabel = (Label)GetNode("MoneyLabel");
		MoneyLabel.Text = $"Money: {this.Money}";
	}

	public void AddSatisfaction(int numb)
	{
		CustomerSatisfactionTotal += numb;
		_customer_satisfactions.Enqueue(numb);

		if(_customer_satisfactions.Count > 49)
			CustomerSatisfactionTotal -= _customer_satisfactions.Dequeue();
		
		SatisfactionRating = 0;
		AverageSatisfactionLabel.Text = $"Rating: {SatisfactionRating}";

		Spawner.ChangeWaitTime();
	}

	public float CalculateCustomersPerMinute()
	{
		float offlineCPM = 0;
		foreach(FoodStall foodStall in Restaurants)
			offlineCPM += Math.Min(foodStall.CustomersPerMinute, Spawner.CustomersPerMinute/Restaurants.Count);

		
		
		CPMLabel.Text = $"{Spawner.CustomersPerMinute.ToString("F2")} Cus/min";
		return offlineCPM;
	}

	public Tuxdollar CalculateMoneyPerMinute()
	{
		Tuxdollar avgMoneyPerDish = new Tuxdollar(0);

		if(Restaurants.Count == 0)
			return avgMoneyPerDish;

		int dishesCount = 0;
		foreach(FoodStall foodStall in Restaurants)
		{
			Tuxdollar totalDishesCost = new Tuxdollar(0);
			foreach(Dish dish in foodStall.Dishes)
			{
				if(dish.Unlocked)
				{	
					totalDishesCost += dish.MealPrice*foodStall.Multiplicator;
					dishesCount++;
				}
			}
			avgMoneyPerDish += totalDishesCost;
		}
		avgMoneyPerDish /= dishesCount;

		return avgMoneyPerDish * CustomersPerMinute;
	}

	private void _on_RecipeButton_pressed()
	{
		TheRecipeBook.OpenRecipeBook();
		RecipeButton.Visible = false;
	}

	private void _on_RecipeBook_popup_hide()
	{
		RecipeButton.Visible = true;
	}

	private void _on_Button_pressed()
	{
		BuildMode = !BuildMode;
		RecipeButton.Visible = !BuildMode;
		
		foreach(Spatial n3d in Spots)
			n3d.Visible = !n3d.Visible;
	}

	public Dictionary<object, object> Save()
	{
		return new Dictionary<object, object>()
		{
            {"Filename", Filename},
			{"StartMoneyValue", Money.Value},
			{"StartMoneyMagnitude", Money.Magnitude},
            {"CustomerSatisfactionTotal", CustomerSatisfactionTotal},
			{"_customer_satisfactionsArray", _customer_satisfactions.ToArray()},
			{"_satisfactionRating", _satisfactionRating},
			{"UnixTimestamp", (int)Time.GetUnixTimeFromSystem()}
			
		};
	}

	public void SaveGame()
	{
		File saveGame = new File();
		saveGame.Open($"user://{(long)Time.GetUnixTimeFromSystem()}.save", File.ModeFlags.Write);

		Godot.Collections.Array saveNodes = GetTree().GetNodesInGroup("Persist");
		saveGame.StoreLine(JSON.Print(Save()));
		foreach(Node saveNode in saveNodes)
			saveGame.StoreLine(JSON.Print(saveNode.Call("Save")));

		saveGame.Close();
	}

	public void LoadGame()
	{
		File saveGame = new File();

		string filePath = GetLastValidSavefile();

		if(filePath == "")
			return;

		var saveNodes = GetTree().GetNodesInGroup("Persist");

		foreach(Node saveNode in saveNodes)
			saveNode.QueueFree();

		saveGame.Open(filePath, File.ModeFlags.Read);
		
		while(saveGame.GetPosition() < saveGame.GetLen())
		{
			Godot.Collections.Dictionary currentLine = (Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result;

			if(currentLine == null)
				continue;

			Node newObject;

			if((String)currentLine["Filename"] == Filename)
			{
				newObject = this;
				Godot.Collections.Array _customer_satisfactionsArray = (Godot.Collections.Array)currentLine["_customer_satisfactionsArray"];
				foreach(System.Single x in _customer_satisfactionsArray)
					_customer_satisfactions.Enqueue((int)x);

				//_offline_seconds = Time.GetUnixTimeFromSystem() - (int)currentLine["UnixTimestamp"];
				Set("UnixTimestamp", currentLine["UnixTimestamp"]);

				_offline_seconds = Time.GetUnixTimeFromSystem() - UnixTimestamp;

				this.Chairs = new List<Chair>();
				this.Spots = new List<Spatial>();
				this.Restaurants = new List<FoodStall>();
			}
			else 
			{
				PackedScene newObjectScene = (PackedScene)ResourceLoader.Load(currentLine["Filename"].ToString());
			    newObject = newObjectScene.Instance();

				if(newObject is Spatial newSpatial)
				{
					newSpatial.Transform = new Transform(newSpatial.Transform.basis, new Vector3((float)currentLine["PositionX"],(float)currentLine["PositionY"],(float)currentLine["PositionZ"]));
			    	newSpatial.Rotation = new Vector3(0, (float)currentLine["RotationY"], 0);
				} 		
			} 
		
			foreach(string key in currentLine.Keys)
			{
				if(key == "Filename" || key == "Parent" || key == "PositionX" || key == "PositionY" || key == "PositionZ" || key == "RotationY" || key == "_customer_satisfactionsArray")
					continue;
				if(key == "ExportScene" && newObject is Spatial newSpatial)
				{
					newSpatial.Visible = false;
					newSpatial.Set(key, GD.Load<PackedScene>(currentLine[key].ToString()));
					continue;
				}
				newObject.Set(key, currentLine[key]);
			}
			if(currentLine["Filename"].ToString() != Filename)
				GetNode(currentLine["Parent"].ToString()).AddChild(newObject);
			
		}
		saveGame.Close();
	}

	public string GetLastValidSavefile()
	{
		double currentUnixTime = Time.GetUnixTimeFromSystem();
		string lastValidSavefilePath = "";
		double lastValidSavefilePathUnixtime = 0;
		Directory dir = new Directory();

		dir.Open("user://");
		dir.ListDirBegin(true, true);
		
		string filename = dir.GetNext();

		while(filename != "")
		{
			if(filename.Find(".save") == -1)
			{
				filename = dir.GetNext();
				continue;
			}

			double fileUnixtime = Convert.ToDouble(filename.Remove(filename.Length-5));

			if(fileUnixtime > currentUnixTime)
			{
				dir.Remove(filename);
				filename = dir.GetNext();
				continue;
			}

			if(fileUnixtime > lastValidSavefilePathUnixtime)
			{
				lastValidSavefilePath = filename;
				lastValidSavefilePathUnixtime = fileUnixtime;
			}

			filename = dir.GetNext();
		}

		return "user://" + lastValidSavefilePath;
	}

	public void DeletSavefiles()
	{
		Directory dir = new Directory();

		dir.Open("user://");
		dir.ListDirBegin(true, true);
		
		string filename = dir.GetNext();

		while(filename != "")
		{
			if(filename.Find(".save") == -1)
			{
				filename = dir.GetNext();
				continue;
			}

			dir.Remove(filename);

			filename = dir.GetNext();
		}
	}

    public override void _Notification(int what)
    {
        if(what == MainLoop.NotificationWmGoBackRequest)
			SaveGame();
    }    

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventKey)
		{
			if(@event.AsText() == "F2" && !@event.IsPressed())
				SaveGame();
			
			if(@event.AsText() == "F3" && !@event.IsPressed())
			{
				DeletSavefiles();
			}
		}
	}
}

public enum InputState
{
	Default,
	Zooming,
	Dragging,
	PopUpOpened,
	MiniGameOpened
}
