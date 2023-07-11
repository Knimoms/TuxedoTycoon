using Godot;
using System;

public class FoodSpwn2D : Node2D
{
    public Minigame2D minigame2D;
    public Texture finishedFoodPNG;
    private Timer _timer;
    private float _timeForCooking = 2;
    private float _timeLeft;
    public TextureProgress TimeBar;
    


	public override void _Ready()
    {
        TimeBar = GetParent().GetNode<TextureProgress>("TextureProgress");
        _timer = GetParent().GetNode<Timer>("Timer");
        minigame2D = GetParent<Minigame2D>();
        finishedFoodPNG = GetParent<Minigame2D>().GetNode<Sprite>("FinishedFood").Texture;
        _timer.WaitTime = 1f;

    }
    private void _on_Timer_timeout()
    {
        {
            if(TimeBar.Value == _timeForCooking)
            {
                minigame2D.CompareLists();
                _timer.Stop();
                TimeBar.Visible = false;
            }
            //finishedFoodPNG = minigame2D.MyFoodStall.OrderedDish.ODpng;
            else
            {
                TimeBar.Value += 1f;
            }
        }
    }
    private void _on_Area2D_input_event(Node Viewport, InputEvent @event, int shape_idx)
	{
		if(@event is InputEventMouseButton && @event.IsPressed())
        {
            if(minigame2D.ingredientList.Count == 0){
                return;
            } else {
                TimeBar.Visible = true;
                TimeBar.Value = 0f;
                _timer.Start();
            }   
        }
	}

}
