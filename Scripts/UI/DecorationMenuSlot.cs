using Godot;
using System.Collections.Generic;

public class DecorationMenuSlot : HBoxContainer
{

    public Decoration Decoration; 

    private Label _price_label;
    public static BaseScript BaseScript;

    public Button Button;
    public static Menu Menu;

    public override void _Ready()
    {

        _price_label = (Label)GetNode("Label");
        if(BaseScript == null)  
            BaseScript = (BaseScript)GetViewport().GetNode("Spatial");
        if(Menu == null)    
            Menu = (Menu)BaseScript.GetNode("UI/Menu");

        Button = (Button)GetNode("Button");
        Menu.decorationMenuSlots.Add(this);
        Decoration.Cost = new Tuxdollar(Decoration.CostValue, Decoration.CostMagnitude);
        _price_label.Text = Decoration.Cost.ToString();
        BaseScript.MoneyTransfered += CheckButtonMode;
    }

    private void _on_Button_pressed()
    {
        if((int)BaseScript.IState >= 5)
            return;

        BaseScript.ActiveDecorationSlot = this;
        BaseScript.UIContainer.Visible = false;

        foreach(DecorSpot decorSpot in BaseScript.DecorSpots)
            decorSpot.Visible = true;

        foreach (Spatial spot in BaseScript.Spots)
            spot.Visible = false;
    }

    public void _on_DecorationMenuSlot_tree_exiting()
    {
        GD.Print("uuh");
    }

    public void CheckButtonMode()
    {
        Button.Disabled = BaseScript.Money < Decoration.Cost;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
