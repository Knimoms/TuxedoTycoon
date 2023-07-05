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
	private float _satisfactionRation;
	public float SatisfactionRating {
		get => _satisfactionRation;
		set{_satisfactionRation = CustomerSatisfactionTotal/_customer_satisfactions.Count;}
	}

	public List<Chair> Chairs = new List<Chair>();
	public Vector3 SpawnPoint {get; private set;}


	public InputState IState;
	public Tuxdollar Money = new Tuxdollar(0);
	public Label MoneyLabel;
	public List<Spatial> Spots = new List<Spatial>();
	public Timer MaxInputDelay;
	public Vector2 InputPosition;
	public Button BuildButton;
	public Camera BaseCam;
	public List<FoodStall> Restaurants = new List<FoodStall>();
	public AdvertisingManager Advertising;
	public CustomerSpawner Spawner;
	public Button AdvertisementButton;
	public Label AverageSatisfactionLabel;
	public Button RecipeButton;
	public RecipeBook TheRecipeBook;
	public Random rnd = new Random();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach(Spatial n3d in Spots)
			n3d.Visible = false;

		foreach(Node node in GetChildren())
			node.Owner = this;
		
		RecipeButton = (Button)GetNode("RecipeButton");
		SpawnPoint = GetNode<Spatial>("SpawnPoint").Transform.origin;
		MaxInputDelay = (Timer)GetNode("MaxInputDelay");
		MoneyLabel = (Label)GetNode("MoneyLabel");
		BuildButton = (Button)GetNode("Button");
		BaseCam = (Camera)GetNode("pivot").GetNode("Camera");
		Advertising = (AdvertisingManager)GetNode("Panel");
		Advertising.Visible = false;
		Spawner = (CustomerSpawner)GetNode("Spawner");
		AdvertisementButton = (Button)GetNode("AdvertisementButton");
		AverageSatisfactionLabel = (Label)GetNode("AverageSatisfaction");
		TheRecipeBook = (RecipeBook)GetNode("RecipeBook");
		TheRecipeBook.FoodStalls = Restaurants;
		TransferMoney(new Tuxdollar(StartMoneyValue, StartMoneyMagnitude));
		Spawner.ChangeWaitTime();
	}

	public Chair GetRandomFreeChair()
	{
		if(Chairs.Count == 0) return null;
		int i = rnd.Next(0, Chairs.Count), j = i;
		Chair chair = Chairs[i];
		while(chair.Occupied)
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
		MoneyLabel.Text = $"Money: {this.Money}";
		Advertising.CheckButtonMode();
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

	private void _on_RecipeButton_pressed()
	{
		TheRecipeBook.OpenRecipeBook();
	}

	private void _on_Button_pressed()
	{
		BuildMode = !BuildMode;
		AdvertisementButton.Visible = BuildMode;
		RecipeButton.Visible = !RecipeButton.Visible;
		
		foreach(Spatial n3d in Spots)
			n3d.Visible = !n3d.Visible;
	}

	private void _on_AdvertisementButton_pressed()
	{
		Advertising.Popup_();
	}

	public void SaveGame()
	{
		var packedScene = new PackedScene();
		packedScene.Pack(GetTree().CurrentScene);
		ResourceSaver.Save("res://SavedScene/saved_scene.tscn",packedScene);

		File saveGame = new File();
		saveGame.Open("user://Save/savegame.save", File.ModeFlags.Write);

		Godot.Collections.Array saveNodes = GetTree().GetNodesInGroup("Persist");

		foreach(Node saveNode in saveNodes)
		{
			saveGame.StoreLine(JSON.Print(saveNode.Call("Save")));
		}

		saveGame.Close();
	}

	public void LoadGame()
	{
		File saveGame = new File();
		if(!saveGame.FileExists("user://Save/savegame.save"))
			return;

		var saveNodes = GetTree().GetNodesInGroup("Persist");

		foreach(Node saveNode in saveNodes)
			saveNode.QueueFree();

		saveGame.Open("user://Save/savegame.save", File.ModeFlags.Read);

		while(!saveGame.EofReached())
		{
			var currentLine = (Dictionary<string, object>)JSON.Parse(saveGame.GetLine()).Result;

			if(currentLine == null)
				continue;

			
			foreach(KeyValuePair<string, object> entry in currentLine)
			{

			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventKey)
		{
			if(@event.AsText() == "F2" && !@event.IsPressed())
				SaveGame();

			if(@event.AsText() == "F3" && !@event.IsPressed())
			{
				GetTree().ChangeScene("res://SavedScene/saved_scene.tscn");

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
