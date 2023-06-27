using Godot;
using System;


public class Ing2D : Node2D
{
    [Export]
    public Ing ing;

    

    [Export]
    public Texture png;

    private Sprite _mySprite;

    private Minigame2D _minigame2D;


    public override void _Ready()
    {
        _minigame2D = GetParent<Minigame2D>();
        _mySprite = GetNode("Area2D").GetNode<Sprite>("Sprite");
        _mySprite.Texture = png;
        _mySprite.GlobalRotation = 0f;
        
    }

    private void _on_Area2D_input_event(Node Viewport, InputEvent @event, int shape_idx)
	{
		if(@event is InputEventMouseButton && @event.IsPressed())
		{
			_minigame2D.AddIng(ing);
            foreach(Ing a in _minigame2D.ingredientList )
            {
				GD.Print(a);
			}
            GD.Print("");
		}
	}
}


    
