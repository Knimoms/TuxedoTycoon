using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerSpawner : Spatial
{
    [Export]
    public PackedScene CustomerScene;

    [Export]
    public float CustomersPerMinute = 20;

    public float BonusCustomersPerMinute;

    private BaseScript Parent;
    private Random _rnd;
    private Timer _timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {   
        _timer = (Timer)GetNode("Timer");
        _rnd = new Random();
        Parent = (BaseScript)GetParent();
        Owner = Parent;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }

    public void ChangeWaitTime()
    {
        BonusCustomersPerMinute = CustomersPerMinute + Parent.Advertising.AdvertisementScore;
        float satisfactionMultiplicator = (Parent.SatisfactionRating < 34)? 0.67f : (Parent.SatisfactionRating > 65)? 1.33f: 1f ;

        if(Parent.CustomerSatisfactionTotal == 0)
            satisfactionMultiplicator = 1f;
        
        _timer.WaitTime = (60/CustomersPerMinute+BonusCustomersPerMinute)*satisfactionMultiplicator;
    }

    private void _on_Timer_timeout()
    {
        if (Parent.Restaurants.Count == 0) return; 

        FoodStall targetFoodStall = TargetFoodStall();
        if (targetFoodStall == null) return;

        Customer customer = CustomerScene.Instance<Customer>();
        customer.TargetRestaurant = targetFoodStall;
        customer.SpawnPoint = (Spatial)targetFoodStall.GetParent().GetNode("SpawnPoint");
        customer.Transform = customer.SpawnPoint.Transform;
        Parent.AddChild(customer);
    }

    private void _on_SpawnrateEvaluationTimer_timeout()
    {
        if(Parent.SatisfactionRating >= Parent.Advertising.GoodRatingMin)
            CustomersPerMinute *= 1.1f;

        if(Parent.SatisfactionRating <= Parent.Advertising.BadRatingMax)
            CustomersPerMinute *= 0.9f;
    }

    private FoodStall TargetFoodStall()
    {
        FoodStall targetFoodStall = null;
        int smallestCustomerCount = int.MaxValue;

        foreach (FoodStall foodStall in Parent.Restaurants)
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