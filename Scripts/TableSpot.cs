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
	[Export]
	public ShaderMaterial BluePrintMat;
	public static Tuxdollar Cost;
	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;
	private CSGBox _bluePrint;
	private CSGMesh _csgMesh;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
        AddToGroup("Persist");

		if (_base_script == null)
            _base_script = (BaseScript)this.GetParent().GetParent();

		_base_script.MoneyTransfered += CheckButtonMode;
		
		Cost = new Tuxdollar(CostValue, CostMagnitude);

		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");
		_csgMesh = (CSGMesh)GetNode("CSGMesh");

		Table tempTable = ExportScene.Instance<Table>();
		_bluePrint = (CSGBox)tempTable.GetNode("CSGBox").Duplicate();
		tempTable.QueueFree();


		Scale = new Vector3(_bluePrint.Scale.x, 1f, _bluePrint.Scale.z);
		_bluePrint.Visible = false;
		
		AddChild(_bluePrint);
		_bluePrint.MaterialOverride = BluePrintMat;
		_bluePrint.Scale = _bluePrint.Scale/Scale;

		_base_script.Spots.Add(this);
		Visible = false;

		_costLabel.Text = $"Cost: {Cost}";		
		
		Vector3 meshScale = _csgMesh.Scale;
		if(Scale.x < Scale.z)
			meshScale.z *= Scale.x/Scale.z;
		else if(Scale.z < Scale.x)
			meshScale.x *= Scale.z/Scale.x;

		_csgMesh.Scale = meshScale;
	}    

	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left)
			return;

		if(!event1.IsPressed() && _base_script.MaxInputDelay.TimeLeft > 0) 
		{
			_base_script.IsoCam.ZoomTo(GlobalTransform.origin + Vector3.Back, 6f, 0.5f);
			_popupMenu.Popup_();
			_costLabel.Text = Cost.ToString();
		}

	}

	private void _on_PopupMenu_about_to_show()
	{
		_bluePrint.Visible = true;
	}

	private void _on_PopupMenu_popup_hide()
	{
		_bluePrint.Visible = false;
	}

	private void _on_ConfirmationButton_pressed()
	{
		if(_base_script.Money < Cost) return;
		Tuxdollar cost = Cost;
		Cost *= 8;
		_base_script.TransferMoney(-cost);
		Table table = ExportScene.Instance<Table>();
		table.Transform = this.Transform;
		table.Scale = new Vector3(1,1,1);
		_base_script.Spots.Remove(this);
		this.QueueFree();
		GetParent().AddChild(table);

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
        newMat.AlbedoColor = (_base_script.Money < Cost)? new Color(150f/255, 150f/255, 150f/255, 0.1f) : new Color(0, 150f/255, 1f, 0.1f);
		//_csgMesh.MaterialOverride = newMat;
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
