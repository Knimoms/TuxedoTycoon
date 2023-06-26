using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerSpawner : Spatial
{
    [Export]
    public PackedScene CustomerScene;

    [Export]
    public float BaseCustomersPerMinute = 20;

    public float CustomersPerMinute;

    private BaseScript _base_script;
    private Random _rnd;
    private Timer _timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {     
        _timer = (Timer)GetNode("Timer");
        _rnd = new Random();
        _base_script = (BaseScript)GetParent();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }

    public void ChangeWaitTime()
    {
        CustomersPerMinute = BaseCustomersPerMinute + _base_script.Advertising.AdvertisementScore;
        _timer.WaitTime = 60/CustomersPerMinute;
    }

    private void _on_Timer_timeout()
    {
        if (_base_script.Restaurants.Count == 0) return; 

        FoodStall targetFoodStall = TargetFoodStall();
        if (targetFoodStall == null) return;

        Customer customer = CustomerScene.Instance<Customer>();
        customer.TargetRestaurant = targetFoodStall;
        customer.SpawnPoint = (Spatial)targetFoodStall.GetParent().GetNode("SpawnPoint");
        customer.Transform = customer.SpawnPoint.Transform;
        targetFoodStall.Parent.AddChild(customer);
    }

    private FoodStall TargetFoodStall()
    {
        FoodStall targetFoodStall = null;
        int smallestCustomerCount = int.MaxValue;

        foreach (FoodStall foodStall in _base_script.Restaurants)
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