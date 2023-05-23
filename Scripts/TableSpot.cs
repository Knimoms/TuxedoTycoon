using Godot;
using System;

public partial class TableSpot : Spatial
{
	private CourtArea _courtArea;
	[Export]
	public PackedScene TableScene;
	[Export]
	public float CostValue;
	[Export]
	public string CostMagnitude;
	public Tuxdollar Cost;
	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		_courtArea = (CourtArea)this.GetParent().GetParent();	
		
		Cost = new Tuxdollar(CostValue, CostMagnitude);

		// Get references to child nodes
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		Table tempTable = TableScene.Instance<Table>();
		CSGBox tempTableBox = (CSGBox)tempTable.GetNode("CSGBox");
	
		Scale = new Vector3(tempTableBox.Scale.x, 1f, tempTableBox.Scale.z);
		tempTable.QueueFree();

		_courtArea.GetParent<BaseScript>().Spots.Add(this);


		// Set the label text for the Cost label
		_costLabel.Text = $"Cost: {Cost}";		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

	}

	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place")) 
			_popupMenu.PopupCentered();
	}

	private void _on_ConfirmationButton_pressed()
	{
		if(_courtArea.Parent.Money < Cost) return;
		_courtArea.Parent.TransferMoney(-Cost);
		Table table = TableScene.Instance<Table>();
		table.Transform = this.Transform;
		table.Scale = new Vector3(1,1,1);
		_courtArea.Parent.Spots.Remove(this);
		this.QueueFree();
		GetParent().AddChild(table);

		_popupMenu.Hide();
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
			if (_courtArea.Parent.Money < Cost)
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
