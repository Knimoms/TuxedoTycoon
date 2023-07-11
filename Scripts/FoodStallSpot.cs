using Godot;
using System.Collections.Generic;

public partial class FoodStallSpot : Spatial
{

	
	[Export]
	public PackedScene ExportScene;

	public Tuxdollar Cost = new Tuxdollar();

	[Export]
	public PackedScene FoodStallModel;
	public static BaseScript Parent;
	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;
	private ulong _input_time;
	FoodStall rest;
	private Particles _poofParticle;


	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		rest = ExportScene.Instance<FoodStall>();
		Cost = new Tuxdollar(rest.Level1CostValue, rest.Level1CostMagnitude);
		if(Parent == null)
			Parent = (BaseScript)GetParent();
		Parent = (BaseScript)this.GetParent();	

		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		_poofParticle = GetNode<Particles>("PoofParticle");
		_poofParticle.Hide();

		Parent.Spots.Add(this);
		Visible = false;
		//if(Name == "RestaurantSpot") _instantiate_restaurant();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left) 
			return;

		if(!event1.IsPressed() && Parent.MaxInputDelay.TimeLeft > 0) 
		{
			_popupMenu.PopupCentered();	
			_costLabel.Text = $"Cost: {Cost}";
		}
	}

	private void _add_restaurant()
	{
		FoodStall rest = ExportScene.Instance<FoodStall>();
		rest.Transform = new Transform(this.Transform.basis, this.Transform.origin + Vector3.Up);
		rest.Rotation = this.Rotation;
		rest.TimeUpgradeCostValue = 4*Cost.Value;
		rest.TimeUpgradeCostMagnitude = Cost.Magnitude;
		rest.QualityUpgradeCostValue = 4*Cost.Value;
		rest.QualityUpgradeCostMagnitude = Cost.Magnitude;
		Parent.Spots.Remove(this);
		this.QueueFree();
		Parent.AddChild(rest);

	}

	private void _on_ConfirmationButton_pressed()
	{
		if(Parent.Money < Cost) 
			return;
		Parent.TransferMoney(-Cost);
		_add_restaurant();
		
		_popupMenu.Hide();		
	}

	private void _on_CancelButton_pressed()
	{
		_popupMenu.Hide();
	}

	private void _on_FoodStallSpot_tree_exiting()
	{
		Parent.MoneyTransfered -= CheckButtonMode;
	}

	public Dictionary<string, object> Save()
	{
		return new Dictionary<string, object>()
		{
			{"Filename", Filename},
			{"Parent", GetParent().GetPath()},
			{"ExportScene", ExportScene.ResourcePath},
			{"PositionX", Transform.origin.x},
			{"PositionY", Transform.origin.y},
			{"PositionZ", Transform.origin.z},
			{"RotationY", Rotation.y},
		};
	}

	public void CheckButtonMode()
	{
		_confirmationButton.Disabled = Parent.Money < Cost;
	}
}
