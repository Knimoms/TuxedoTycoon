using Godot;
using System;

public class FoodSpwn2D : Node2D
{
    Minigame2D minigame2D;

	public override void _Ready()
    {
        minigame2D = GetParent<Minigame2D>();
    }

    private void _on_Area2D_input_event(Node Viewport, InputEvent @event, int shape_idx)
	{
		if(@event is InputEventMouseButton && @event.IsPressed())
            minigame2D.CompareLists();

	}

}
