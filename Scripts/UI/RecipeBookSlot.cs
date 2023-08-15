using Godot;
using System;

public class RecipeBookSlot : Control
{
    public Dish Dish;
    public Sprite Sprite;
    public static RecipeBook RecipeBook;
    public override void _Ready()
    {
        GD.Print("mau");
        Sprite = (Sprite)GetNode("Sprite");
        if(Dish.DishIcon != null)
            Sprite.Texture = Dish.DishIcon;
    }

    private void _on_RecipeSlot_pressed()
    {
        RecipeBook.CurrentDish = Dish;
    }

    private void _on_Area2D_input_event(Node viewport, InputEvent event1, int shape_idx)
    {
        if(!(event1 is InputEventMouseButton mb) || mb.ButtonIndex != (int)ButtonList.Left) 
			return;

		if(!event1.IsPressed() && BaseScript.DefaultBaseScript.MaxInputDelay.TimeLeft > 0) 
		{
			RecipeBook.CurrentDish = Dish;			
		}
    }
}
