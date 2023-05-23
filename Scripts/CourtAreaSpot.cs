using Godot;
using System;

public partial class CourtAreaSpot : Spatial
{
	private BaseScript _parent;
	[Export]
	public PackedScene CourtAreaScene;
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
		_parent = (BaseScript)this.GetParent();	
		
		Cost = new Tuxdollar(CostValue, CostMagnitude);

		// Get references to child nodes
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		// Set the label text for the Cost label
		_parent.Spots.Add(this);
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
		if(_parent.Money < Cost) return;
		_parent.TransferMoney(-Cost);
		CourtArea courtArea = CourtAreaScene.Instance<CourtArea>();
		courtArea.Transform = this.Transform;
		GetParent().AddChild(courtArea);
		_parent.Spots.Remove(this);
		this.QueueFree();

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
			if (_parent.Money < Cost)
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
