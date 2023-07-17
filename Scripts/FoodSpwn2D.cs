using Godot;
using System;
using System.Collections.Generic;


public class FoodSpwn2D : Node2D
{
    private Sprite _finishedFood;
    public Minigame2D minigame2D;
    private Timer _minigameTimer;
    public FoodStall foodStall;
    public TextureProgress TimeBar;
    


	public override void _Ready()
    {
        TimeBar = GetParent().GetNode<TextureProgress>("TextureProgress");
        _minigameTimer = GetParent().GetNode<Timer>("Timer");
        minigame2D = GetParent<Minigame2D>();
        foodStall = GetParent().GetParent<FoodStall>();
        _finishedFood = GetParent().GetNode<Sprite>("FinishedFood");

    }
    private void _on_Timer_timeout()
    {
        {
            minigame2D.CompareLists();
            minigame2D.ingredientList.Clear();
            //finishedFoodPNG = minigame2D.MyFoodStall.OrderedDish.ODpng;
        }
    }
    private void _on_Area2D_input_event(Node Viewport, InputEvent @event, int shape_idx)
	{
		if(@event is InputEventMouseButton && @event.IsPressed())
        {
            foreach(Sprite fanta in minigame2D.IngSpots)
                fanta.Texture = null;

            GD.Print(minigame2D.MyFoodStall.OrderedDish.GetTree());

            _minigameTimer.WaitTime = foodStall.TimerProp.WaitTime/4f;
            if(minigame2D.ingredientList.Count == 0){
                return;
            } else {
                _finishedFood.Texture = minigame2D.MyFoodStall.OrderedDish.DishIcon ;
                _minigameTimer.Start();
                GD.Print("TimerStarted");
                
            }   
        }
	}
    

}
