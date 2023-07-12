using Godot;
using System;
using System.Collections.Generic;

public partial class TableSpot : Spatial
{
	private BaseScript _base_script;
	[Export]
	public PackedScene ExportScene;
	[Export]
	public float CostValue;
	[Export]
	public string CostMagnitude;
	public static Tuxdollar Cost;
	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;
	private MeshInstance _meshInstance;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
        AddToGroup("Persist");

		if (_base_script == null)
            _base_script = (BaseScript)this.GetParent().GetParent();

		_base_script.MoneyTransfered += CheckButtonMode;
		
		Cost = new Tuxdollar(CostValue, CostMagnitude);

		// Get references to child nodes
		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");
		_meshInstance = (MeshInstance)GetNode("MeshInstance");

		Table tempTable = ExportScene.Instance<Table>();
		CSGBox tempTableBox = (CSGBox)tempTable.GetNode("CSGBox");
	
		Scale = new Vector3(tempTableBox.Scale.x, 1f, tempTableBox.Scale.z);
		tempTable.QueueFree();

		_base_script.Spots.Add(this);
		Visible = false;


		// Set the label text for the Cost label
		_costLabel.Text = $"Cost: {Cost}";		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

	}

	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left)
			return;

		if(!event1.IsPressed() && _base_script.MaxInputDelay.TimeLeft > 0) 
		{
			_popupMenu.Popup_();
			_costLabel.Text = Cost.ToString();
		}

	}

	private void _on_ConfirmationButton_pressed()
	{
		if(_base_script.Money < Cost) return;
		_base_script.TransferMoney(-Cost);
		Table table = ExportScene.Instance<Table>();
		table.Transform = this.Transform;
		table.Scale = new Vector3(1,1,1);
		_base_script.Spots.Remove(this);
		this.QueueFree();
		GetParent().AddChild(table);

		Cost *= 8;

		_popupMenu.Hide();
	}

	private void _on_CancelButton_pressed()
	{
		_popupMenu.Hide();
	}

	public void CheckButtonMode()
	{
		_confirmationButton.Disabled = _base_script.Money < Cost;
		SpatialMaterial newMat = new SpatialMaterial();
        newMat.AlbedoColor = (_base_script.Money < Cost)? new Color(150f/255, 150f/255, 150f/255, 1) : new Color(0, 150f/255, 255f/255, 1);
		_meshInstance.MaterialOverride = newMat;
	}

	public PopupMenu GetPopupMenu()
    {
        return _popupMenu;
    }

	private void _on_TableSpot_tree_exiting()
	{
		_base_script.MoneyTransfered -= CheckButtonMode;
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
}
