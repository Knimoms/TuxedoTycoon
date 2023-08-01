using Godot;
using System.Collections.Generic;

public partial class FoodStallSpot : Spatial
{

	
	[Export]
	public PackedScene ExportScene;

	public Tuxdollar Cost = new Tuxdollar();

	[Export]
	public PackedScene FoodStallModel;

	[Export]
	public ShaderMaterial BlueprintShader;
	[Export]
	public Color DeactivedColor;
	public static BaseScript Parent;
	private PopupMenu _popupMenu;
	private Label _costLabel;
	private Button _confirmationButton;
	private ulong _input_time;
	FoodStall rest;
	private MeshInstance _blueprint;
	private Spatial _spot;
	private Particles poofParticleInstance;

	private ShaderMaterial _my_blueprint_shader;
	private Color defaultColor;

	


	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		defaultColor = (Color)BlueprintShader.GetShaderParam("color");
		rest = ExportScene.Instance<FoodStall>();
		Cost = new Tuxdollar(rest.Stage1CostValue, rest.Stage1CostMagnitude);
		if(Parent == null)
			Parent = (BaseScript)GetParent();
		Parent = (BaseScript)this.GetParent();
		Parent.MoneyTransfered += CheckButtonMode;	

		_popupMenu = GetNode<PopupMenu>("PopupMenu");
		
		_popupMenu.PopupCentered();
		_popupMenu.Hide();

		_costLabel = _popupMenu.GetNode<Label>("CostLabel");
		_confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");

		_spot = (Spatial)GetNode("Spot");

		string[] splittedFilename = rest.Filename.Split('/');
		splittedFilename[splittedFilename.Length-1] = null;
		string FolderPath = string.Join("/", splittedFilename);

		_my_blueprint_shader = (ShaderMaterial)BlueprintShader.Duplicate();

		Spatial blueprintGrandpa = (Spatial)GD.Load<PackedScene>(FolderPath+"/Stages/Stage1.tscn").Instance();
		blueprintGrandpa.RotationDegrees = new Vector3(0, -90, 0);
		_blueprint = (MeshInstance)blueprintGrandpa.GetChild(0).GetChild(0);
		_blueprint.MaterialOverride = _my_blueprint_shader;

		_spot.AddChild(blueprintGrandpa);

		Parent.Spots.Add(_spot);
		Visible = true;
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
		rest.Transform = new Transform(this.Transform.basis, this.Transform.origin);
		rest.Rotation = this.Rotation;
		rest.LevelUpCostValue = 4 * Cost.Value;
		rest.LevelUpCostMagnitude = Cost.Magnitude;
		Parent.Spots.Remove(this);
		this.QueueFree();
		Parent.AddChild(rest);
	}

	private void _on_ConfirmationButton_pressed()
	{
		if(Parent.Money < Cost) 
			_confirmationButton.Disabled = true;
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
		Color temp = (Parent.Money < Cost)? DeactivedColor : defaultColor;
        _my_blueprint_shader.SetShaderParam ("color", temp);
	}
}
