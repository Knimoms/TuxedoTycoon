using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerSpawner : Spatial
{
    [Export]
    public PackedScene CustomerScene;
    private Random _rnd;
    private Timer _timer;
    public List<FoodStall> Rests;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {     
        _timer = (Timer)GetNode("Timer");
        _rnd = new Random();
        Rests = new List<FoodStall>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }

    public void Change_wait_time()
    {
        float avgTimeClutter = 0;
        foreach(FoodStall r in Rests)
        {
            avgTimeClutter += r.TimerProp.WaitTime;
        }

        _timer.WaitTime = (avgTimeClutter / Rests.Count) / Rests.Count/ 1.5f;
        if (Rests.Count == 1)
        {
            _timer.Start();
            _on_Timer_timeout();
        }
    }

    private void _on_Timer_timeout()
    {
        if (Rests.Count == 0) return;

        FoodStall targetFoodStall = TargetFoodStall();
        if (targetFoodStall == null) return;

        Customer customer = CustomerScene.Instance<Customer>();
        customer.TargetRestaurant = targetFoodStall;
        customer.Transform = Transform;
        GetParent().AddChild(customer);
        customer.SpawnPoint = GlobalTransform.origin;
    }

    private FoodStall TargetFoodStall()
    {
        FoodStall targetFoodStall = null;
        int smallestCustomerCount = int.MaxValue;

        foreach (FoodStall foodStall in Rests)
        {
            int customerCount = foodStall.IncomingCustomers.Count;
            if (customerCount < smallestCustomerCount)
            {
                targetFoodStall = foodStall;
                smallestCustomerCount = customerCount;
            }
        }

        return targetFoodStall;
    }
}