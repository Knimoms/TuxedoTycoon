using Godot;
using System;
using System.Collections.Generic;



public class FoodSpwn2D : Node2D
{
    public Minigame2D minigame2D;
    private Timer _minigameTimer;
    public FoodStall foodStall;
    public TextureProgress TimeBar;
    public Button closeButton;
    private float _timeLeft;
    public List<Ing2D> ing2DList = new List<Ing2D>();
    public int i = 1;


	public override void _Ready()
    {
        TimeBar = GetParent().GetNode<TextureProgress>("TextureProgress");
        _minigameTimer = GetParent().GetNode<Timer>("Timer");
        minigame2D = GetParent<Minigame2D>();
        foodStall = GetParent().GetParent<FoodStall>();
        _minigameTimer.WaitTime = foodStall.TimerProp.WaitTime/4f;
        closeButton = GetParent().GetNode<Button>("CloseButton");
        while(GetParent().GetNodeOrNull<Ing2D>($"Ing2D"+ i) != null)
        {
            ing2DList.Add(GetParent().GetNode<Ing2D>($"Ing2D"+i));
            i++;
        }
    }

    public override void _Process(float delta)
    {
        {
            if (_minigameTimer.TimeLeft > 0f)
            {
                _timeLeft = ((1 - _minigameTimer.TimeLeft / _minigameTimer.WaitTime) * 100);
                TimeBar.Value = _timeLeft;
            }
        }
    }
    private void _on_Timer_timeout()
    {
        {
            foodStall.CookingSound.Stop();
            TimeBar.Visible = false;
            minigame2D.CompareLists();
            minigame2D.ingredientList.Clear();
            minigame2D.Order.Texture = null;
        }
    }
    private void _on_Area2D_input_event(Node Viewport, InputEvent @event, int shape_idx)
	{
        if(minigame2D.Cooking || minigame2D.ingredientList.Count == 0 || minigame2D.MyFoodStall.IncomingCustomers.Count == 0 || minigame2D.MyFoodStall.IncomingCustomers[0].State != CustomerState.WaitingInQueue)
            return;
		if(@event is InputEventMouseButton && @event.IsPressed())
        {
            foodStall.CookingSound.Play();
            minigame2D.TrashButtn.Visible = false;
            TimeBar.Visible = true;
            closeButton.Visible = false;
            minigame2D.Cooking = true;
            foreach(Sprite fanta in minigame2D.IngSpots)
                fanta.Texture = null;

            _minigameTimer.WaitTime = foodStall.TimerProp.WaitTime/4f;
            _minigameTimer.Start();   
             
        }
	}
}
