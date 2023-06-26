using Godot;
using System;
using System.Collections.Generic;

public partial class BaseScript : Spatial
{
	[Export]
	public float StartMoneyValue;
	[Export]
	public string StartMoneyMagnitude;

	public InputState IState;
	public Tuxdollar Money = new Tuxdollar(0);
	public Label MoneyLabel;

	public List<Spatial> Spots = new List<Spatial>();

	public Timer MaxInputDelay;

	public Vector2 InputPosition;

	public Button BuildButton;
	public bool BuildMode = false;
	public bool MiniGameStarted = false;

	public Camera BaseCam;
	public List<FoodStall> Restaurants = new List<FoodStall>();

	public AdvertisingManager Advertising;
	public CustomerSpawner Spawner;

	public Button AdvertisementButton;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach(Spatial n3d in Spots)
			n3d.Visible = false;
		
		MaxInputDelay = (Timer)GetNode("MaxInputDelay");
		MoneyLabel = (Label)GetNode("MoneyLabel");
		BuildButton = (Button)GetNode("Button");
		BaseCam = (Camera)GetNode("pivot").GetNode("Camera");
		Advertising = (AdvertisingManager)GetNode("Panel");
		Advertising.Visible = false;
		Spawner = (CustomerSpawner)GetNode("Spawner");
		AdvertisementButton = (Button)GetNode("AdvertisementButton");
		TransferMoney(new Tuxdollar(StartMoneyValue, StartMoneyMagnitude));
		Spawner.ChangeWaitTime();
	}

	public void TransferMoney(Tuxdollar Money)
	{
		this.Money += Money;
		MoneyLabel.Text = $"Money: {this.Money}";
		Advertising.CheckButtonMode();
	}

	private void _on_Button_pressed()
	{
		BuildMode = !BuildMode;
		AdvertisementButton.Visible = BuildMode;
		
		foreach(Spatial n3d in Spots)
			n3d.Visible = !n3d.Visible;
	}

	private void _on_AdvertisementButton_pressed()
	{
		Advertising.Popup_();
	}

	public void _on_Upgrade_pressed()
	{

	}
}

public enum InputState
{
	Default,
	Zooming,
	Dragging,
	UIopened,
	MiniGameOpened
}
