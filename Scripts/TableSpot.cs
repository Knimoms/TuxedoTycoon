using Godot;
using System;

public partial class TableSpot : Node3D
{
	private CourtArea _parent;
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
		_parent = (CourtArea)this.GetParent();	
		
		Cost = new Tuxdollar(CostValue, CostMagnitude);

		// Get references to child nodes
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		Table tempTable = TableScene.Instantiate<Table>();
		CsgBox3D tempTableBox = (CsgBox3D)tempTable.GetNode("CSGBox3D");
	
		Scale = new Vector3(tempTableBox.Scale.X, 1f, tempTableBox.Scale.Z);
		tempTable.QueueFree();

		// Set the label text for the Cost label
		_costLabel.Text = $"Cost: {Cost}";		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place") && !_popupMenu.Visible) 
			_popupMenu.Popup();
	}

	private void _on_confirmation_button_pressed()
	{
		if(_parent.Parent.Money < Cost) return;
		_parent.Parent.TransferMoney(-Cost);
		Table table = TableScene.Instantiate<Table>();
		table.Position = this.Position;
		table.Rotation = this.Rotation;
		this.QueueFree();
		_parent.AddChild(table);

		_popupMenu.Hide();
	}

	private void _on_cancel_button_pressed()
	{
		// Hide the PopupMenu
		_popupMenu.Hide();
	}

	 public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motionEvent)
		{
			if (_parent.Parent.Money < Cost)
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
