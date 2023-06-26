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

        currentLevel = 1;
        Cost = CalculateCost(currentLevel);
        UpdateCostLabel();

        _confirmationButton.Text = "Upgrade";
        _costLabel.Text = $"Cost: {Cost}";

        CreateChairs();

        // Check initial money and disable the button if not enough funds
        if (_parent.Parent.Money < Cost)
            _confirmationButton.Disabled = true;
    }

    private void UpdateCostLabel()
    {
        _costLabel.Text = $"Cost: {Cost}";
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
        int chairCount = currentLevel - 1;

        for (int i = 0; i < chairCount; i++)
        {
            Chair chair = new Chair();
            _parent.Chairs.Add(chair);
            AddChild(chair);
        }
    }

    private void _on_ConfirmationButton_pressed()
    {
        if (_parent.Parent.Money < Cost)
        {
            _confirmationButton.Disabled = true;
            return;
        }

        _parent.Parent.TransferMoney(-Cost);

        currentLevel++;
        Cost = CalculateCost(currentLevel);
        UpdateCostLabel();

        Chair chair = new Chair();
        _parent.Chairs.Add(chair);
        AddChild(chair);
    }

    private void _on_CancelButton_pressed()
    {
        _popupMenu.Hide();
    }

    private Tuxdollar CalculateCost(int level)
    {
        int cost = 7000 * (int)Mathf.Pow(10, level - 1);
        return new Tuxdollar(cost);
    }

    public override void _Process(float delta)
    {
    }
}
