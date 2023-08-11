using Godot;
using System;

public class Decoration : Spatial
{
    [Export]
    public float CostValue;
    [Export]
    public string CostMagnitude;

    [Export]
    public int SatisfactionBonus;

    public Tuxdollar Cost{get; private set;}

    private BaseScript _base_script;

    public override void _Ready()
    {
        AddToGroup("Persist");
        Cost = new Tuxdollar(CostValue, CostMagnitude);
        _base_script = (BaseScript)GetViewport().GetNode("Spatial");

        _base_script.SatisfactionBonus += SatisfactionBonus;

        Connect("tree_exiting", this, nameof(_on_Decoration_tree_exiting));
    }

    private void _on_Decoration_tree_exiting()
    {
        _base_script.SatisfactionBonus -= SatisfactionBonus;
    }




//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
