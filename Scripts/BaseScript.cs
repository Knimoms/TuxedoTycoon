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
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach(Spatial n3d in Spots)
			n3d.Visible = false;
		
		MaxInputDelay = (Timer)GetNode("MaxInputDelay");
		this.MoneyLabel = (Label)GetNode("MoneyLabel");
		this.BuildButton = (Button)GetNode("Button");
		this.BaseCam = (Camera)GetNode("pivot").GetNode("Camera");
		TransferMoney(new Tuxdollar(StartMoneyValue, StartMoneyMagnitude));
	}

	public void TransferMoney(Tuxdollar Money)
	{
		this.Money += Money;
		MoneyLabel.Text = $"Money: {this.Money}";
	}

	public void _on_Button_pressed()
	{
		BuildMode = !BuildMode;

		foreach(Spatial n3d in Spots)
			n3d.Visible = !n3d.Visible;
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
