using Godot;
using System;


public class Ing2D : Node2D
{
    [Export]
    public Ingredient ing;

    private Sprite _mySprite;

    private Minigame2D _minigame2D;


    public override void _Ready()
    {
        _minigame2D = GetParent<Minigame2D>();
        _mySprite = GetNode("Area2D").GetNode<Sprite>("Sprite");
        _mySprite.Texture = Dish.GetIngredientSprite(ing);
        _mySprite.GlobalRotation = 0f;
        
    }

    private void _on_Area2D_input_event(Node Viewport, InputEvent @event, int shape_idx)
	{
		if(@event is InputEventMouseButton && @event.IsPressed())
		{
			Sprite positionSprite = _minigame2D.AddIng(ing);

            if(positionSprite != null)
                positionSprite.Texture = Dish.GetIngredientSprite(ing);

		}
	}
}


    
