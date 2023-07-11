using Godot;
using System;
using System.Collections.Generic;

public partial class Table : Spatial
{
    private int chairsCount;
    public Tuxdollar Cost;
    private int currentLevel = 0;

    public BaseScript _base_script;
    public string CostMagnitude;
    private PopupMenu _popupMenu;
    private Label _costLabel;
    private Button _confirmationButton;
    private Button _cancelButton;
    private int maxPossibleChairs = 16;

    public override void _Ready()
    {
        this.AddToGroup("Persist");
        GetParent<NavigationMeshInstance>().BakeNavigationMesh(false);

        if (_base_script == null)
            _base_script = (BaseScript)this.GetParent().GetParent();
        
        _base_script.MoneyTransfered += CheckButtonMode;

        _popupMenu = GetNode<PopupMenu>("PopupMenu");
        _costLabel = _popupMenu.GetNode<Label>("CostLabel");
        _confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");
        _cancelButton = _popupMenu.GetNode<Button>("CancelButton");

        _popupMenu.Hide();

        Cost = CalculateCost(currentLevel);
        UpdateCostLabel();

        _confirmationButton.Text = "Buy a chair";
        _costLabel.Text = $"Cost: {Cost}\nChairs: {currentLevel}";

        CreateChairs();

        CheckButtonMode();

        for (int i = 0; i <= maxPossibleChairs; i++)
        {
            Chair chair = GetNodeOrNull<Chair>($"Chair{i}");
            if (chair != null)
                chairsCount++;
        }

        if(currentLevel == 0) LevelUp();
    }

    private void UpdateCostLabel()
    {
        _costLabel.Text = $"Cost: {Cost}\nChairs: {currentLevel}";
    }

    private void LevelUp()
    {
        if(_base_script.Money >= Cost)
        {
            _base_script.TransferMoney(-Cost);

            currentLevel++;
            Cost = CalculateCost(currentLevel);
            UpdateCostLabel();

            CreateChairs();
        }
    }

    public void CheckButtonMode()
    {
        _confirmationButton.Disabled = _base_script.Money < Cost || currentLevel == chairsCount;
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 position, Vector3 normal, int shape_idx)
    {
        if (!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left || !_base_script.BuildMode)
            return;

        if (!event1.IsPressed() && _base_script.MaxInputDelay.TimeLeft > 0)
        {
            _popupMenu.PopupCentered();
        }
    }

    private void CreateChairs()
    {

        if (currentLevel > 0)
        {
            Chair chair = GetNodeOrNull<Chair>($"Chair{currentLevel}");
            if (chair != null)
                chair.MakeUsable();
        }
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
			{"currentLevel", currentLevel}
		};
	}

    private Tuxdollar CalculateCost(int level)
    {
        ulong cost = 7000 * (ulong)Mathf.Pow(10, level);
        return new Tuxdollar(cost);
    }
}
