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
	public ShaderMaterial BlueprintShader;
	[Export]
	public Color DeactivedColor;

	private Color defaultColor;
	private Color _bluePrintColor;
	public static Tuxdollar Cost;
	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;
	private Spatial _blue_print_container;
	private MeshInstance[] _blue_print;
	private MeshInstance _mesh;
	private Color _defaultColor;
	private ShaderMaterial _my_blueprint_shader;
	private ShaderMaterial _circle_shader;

	private int _sizeMultiplicator = 1;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	defaultColor = (Color)BlueprintShader.GetShaderParam("color");
        AddToGroup("Persist");

		if (_base_script == null)
            _base_script = (BaseScript)this.GetParent().GetParent();

		_base_script.MoneyTransfered += CheckButtonMode;
		
		Cost = new Tuxdollar(CostValue, CostMagnitude);

		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");
		_mesh = (MeshInstance)GetNode("MeshInstance");

		_circle_shader = (ShaderMaterial)_mesh.GetSurfaceMaterial(0);

		Table tempTable = ExportScene.Instance<Table>();
		_sizeMultiplicator = (int)tempTable.TableSize + 1;
		_blue_print_container = (Spatial)tempTable.GetNode("Spatial").Duplicate();
		AddChild(_blue_print_container);
		Godot.Collections.Array temp = _blue_print_container.GetChildren();
		_blue_print = new MeshInstance[temp.Count];
		for (int i = 0; i < _blue_print.Length; i++)
			_blue_print[i] = (MeshInstance)temp[i];

		tempTable.QueueFree();


		Scale = new Vector3(_blue_print_container.Scale.x, 1f, _blue_print_container.Scale.z);
		_blue_print_container.Visible = false;
		
		_my_blueprint_shader = (ShaderMaterial)BlueprintShader.Duplicate();
		foreach(MeshInstance meshInstance in _blue_print)
		{
			meshInstance.MaterialOverride = _my_blueprint_shader;
			meshInstance.Scale = meshInstance.Scale/Scale;
		}

		_base_script.Spots.Add(this);
		Visible = false;

		_costLabel.Text = $"Cost: {Cost*_sizeMultiplicator}";		
		
		Vector3 meshScale = _mesh.Scale;
		if(Scale.x < Scale.z)
			meshScale.z *= Scale.x/Scale.z;
		else if(Scale.z < Scale.x)
			meshScale.x *= Scale.z/Scale.x;

		_mesh.Scale = meshScale;
	}    

	private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left)
			return;

		if(!event1.IsPressed() && _base_script.MaxInputDelay.TimeLeft > 0) 
		{
			_base_script.IsoCam.ZoomTo(GlobalTransform.origin + Vector3.Back, 6f, 0.5f);
			_popupMenu.Popup_();
			_costLabel.Text = (Cost*_sizeMultiplicator).ToString();
		}

	}

	private void _on_PopupMenu_about_to_show()
	{
		_blue_print_container.Visible = true;
	}

	private void _on_PopupMenu_popup_hide()
	{
		_blue_print_container.Visible = false;
	}

	private void _on_ConfirmationButton_pressed()
	{
		if(_base_script.Money < Cost*_sizeMultiplicator) return;
		Tuxdollar cost = Cost*_sizeMultiplicator;
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
		Color temp = (_base_script.Money < Cost*_sizeMultiplicator)? DeactivedColor : defaultColor;
        _my_blueprint_shader.SetShaderParam ("color", temp);
		_circle_shader.SetShaderParam("deactivated", _confirmationButton.Disabled);
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
			 {"CostValue",Cost.Value},
			 {"CostMagnitude", Cost.Magnitude}
		};
	}
}
