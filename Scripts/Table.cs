using Godot;
using System;
using System.Collections.Generic;

public partial class Table : Spatial
{
    [Export]
    public Size TableSize;

    private int chairsCount;
    public Tuxdollar Cost;

    public BaseScript _base_script;
    public string CostMagnitude;
    private PopupMenu _popupMenu;
    private Label _costLabel;
    private Button _confirmationButton;
    private Button _cancelButton;

    public override void _Ready()
    {
        this.AddToGroup("Persist");
        chairsCount = GetChildCount();

        if (_base_script == null)
            _base_script = (BaseScript)this.GetParent().GetParent();
        
        _popupMenu = GetNode<PopupMenu>("PopupMenu");
        _costLabel = _popupMenu.GetNode<Label>("CostLabel");
        _confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");
        _cancelButton = _popupMenu.GetNode<Button>("CancelButton");
        
        _popupMenu.Hide();


        _confirmationButton.Text = "Buy a chair";
        Godot.Collections.Array chairs = GetChildren();

        foreach(object obj in chairs)
        {
            if(obj is Chair chair)
                chair.MakeUsable();
        }
    }

    private void LevelUp()
    {
        if(_base_script.Money >= Cost)
        {
            Tuxdollar cost = Cost;

            _base_script.TransferMoney(-cost);

        }
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 position, Vector3 normal, int shape_idx)
    {
        
    }

    private void _on_ConfirmationButton_pressed()
    {
        LevelUp();
    }

    private void _on_CancelButton_pressed()
    {
        _popupMenu.Hide();
    }

    public Dictionary<string, object> Save()
	{
		return new Dictionary<string, object>()
		{
            {"Filename", Filename},
			{"Parent", GetParent().GetPath()},
			{"PositionX", Transform.origin.x},
			{"PositionY", Transform.origin.y},
			{"PositionZ", Transform.origin.z},
            {"RotationY", Rotation.y},
		};
	}

    private Tuxdollar CalculateCost(int level)
    {
        ulong cost = 7000 * (ulong)Mathf.Pow(10, level);
        return new Tuxdollar(cost);
    }

    public enum Size
    {
        Small,
        Medium,
        Big
    }
}
