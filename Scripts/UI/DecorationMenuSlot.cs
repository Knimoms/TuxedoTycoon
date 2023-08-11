using Godot;
using System;

public class DecorationMenuSlot : HBoxContainer
{

    public Decoration Decoration; 

    private Label _price_label;
    public static BaseScript BaseScript;

    public static Menu Menu;


    public override void _Ready()
    {
        _price_label = (Label)GetNode("Label");
        if(BaseScript == null)  
            BaseScript = (BaseScript)GetViewport().GetNode("Spatial");
        if(Menu == null)    
            Menu = (Menu)BaseScript.GetNode("UI/Menu");;
        _price_label.Text = Decoration.Cost.ToString();
    }

    public override void _Process(float delta)
    {
        if(RectGlobalPosition.y < Menu.Treshold.GlobalPosition.y)
            Visible = false;
        else Visible = true;
    }

    private void _on_Button_pressed()
    {
        BaseScript.BoughtSpatial = Decoration;
        BaseScript.UIContainer.Visible = false;

        foreach(DecorSpot decorSpot in BaseScript.DecorSpots)
            decorSpot.Visible = true;
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
