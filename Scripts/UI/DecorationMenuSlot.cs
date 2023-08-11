using Godot;
using System;

public class DecorationMenuSlot : HBoxContainer
{

    public Decoration Decoration; 

    private Label _price_label;
    public BaseScript BaseScript;


    public override void _Ready()
    {
        _price_label = (Label)GetNode("Label");
        BaseScript = (BaseScript)GetViewport().GetNode("Spatial");
        _price_label.Text = Decoration.Cost.ToString();
    }

    private void _on_Button_pressed()
    {
        BaseScript.BoughtSpatial = Decoration;
        BaseScript.UIContainer.Visible = false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
