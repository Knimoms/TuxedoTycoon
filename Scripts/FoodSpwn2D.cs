using Godot;
using System;

public class FoodSpwn2D : Node2D
{
    Minigame2D minigame2D = new Minigame2D();

    private void _on_Area2D_input_event(Node Viewport, InputEvent @event, int shape_idx)
	{
		if(@event is InputEventMouseButton && @event.IsPressed())
		{
            if(minigame2D.CompareLists())
			{
				GD.Print("NICE");
			}
			else
			{
				GD.Print("No!");
			}
		}
	}

}
