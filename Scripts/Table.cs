using Godot;
using System;
using System.Collections.Generic;

public partial class Table : Spatial
{
    private CourtArea _parent;
    public string CostMagnitude;
    private PopupMenu _popupMenu;
    private Label _costLabel;
    private Button _confirmationButton;
    private Button _cancelButton;
    public Tuxdollar Cost;
    private int currentLevel;
    private List<Chair> usableChairs = new List<Chair>();
    private int chairsCount;
    private int maxPossibleChairs = 16;

    public override void _Ready()
    {
        GetParent<NavigationMeshInstance>().BakeNavigationMesh(false);

        _parent = (CourtArea)this.GetParent().GetParent();

        _popupMenu = GetNode<PopupMenu>("PopupMenu");
        _costLabel = _popupMenu.GetNode<Label>("CostLabel");
        _confirmationButton = _popupMenu.GetNode<Button>("ConfirmationButton");
        _cancelButton = _popupMenu.GetNode<Button>("CancelButton");

        _popupMenu.Hide();

        if (_parent.Parent == null)
            _parent.Parent = _parent.GetParent<BaseScript>();

        currentLevel = 0;
        Cost = CalculateCost(currentLevel);
        UpdateCostLabel();

        _confirmationButton.Text = "Buy a chair";
        _costLabel.Text = $"Cost: {Cost}\nChairs: {currentLevel}";

        CreateChairs();

        if (_parent.Parent.Money < Cost)
            _confirmationButton.Disabled = true;

        for (int i = 0; i <= maxPossibleChairs; i++)
        {
            Chair chair = GetNodeOrNull<Chair>($"Chair{i}");
            if (chair != null)
            {
                chairsCount++;
            }
        }

        LevelUp();
    }

    private void UpdateCostLabel()
    {
        _costLabel.Text = $"Cost: {Cost}\nChairs: {currentLevel}";
    }

    private void LevelUp()
    {
        _parent.Parent.TransferMoney(-Cost);

        currentLevel++;
        Cost = CalculateCost(currentLevel);
        UpdateCostLabel();

        CreateChairs();

        foreach (var chair in usableChairs)
        {
            _parent.Chairs.Add(chair);
        }

        if (_parent.Parent.Money < Cost)
        {
            _confirmationButton.Disabled = true;
            return;
        }
        
        if (currentLevel == chairsCount)
        {
            _confirmationButton.Disabled = true;
            return;
        }
    }

    private void _on_Area_input_event(Node camera, InputEvent event1, Vector3 position, Vector3 normal, int shape_idx)
    {
        if (!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left || !_parent.Parent.BuildMode)
            return;

        if (!event1.IsPressed() && _parent.Parent.MaxInputDelay.TimeLeft > 0)
        {
            _popupMenu.PopupCentered();
        }
    }

    private void CreateChairs()
    {
        usableChairs.Clear();

        if (currentLevel > 0)
        {
            Chair chair = GetNodeOrNull<Chair>($"Chair{currentLevel}");
            if (chair != null)
            {
                chair.MakeUsable();
                usableChairs.Add(chair);
            }
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

    private Tuxdollar CalculateCost(int level)
    {
        ulong cost = 7000 * (ulong)Mathf.Pow(10, level);
        return new Tuxdollar(cost);
    }

    public override void _Process(float delta)
    {
    }
}
