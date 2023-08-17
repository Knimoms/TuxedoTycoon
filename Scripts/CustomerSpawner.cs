using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerSpawner : Spatial
{
    [Export]
    public PackedScene CustomerScene;

    [Export]
    public float CustomersPerMinute;

    private BaseScript Parent;
    private Random _rnd;
    private Timer _timer;
    private Timer _spawnrate_evaluation_timer;

    public float SpawnrateEvaluationTimerWaitTime;
    public float SpawnrateEvaluationTimerTimeLeft;

    public Spatial[] SpawnPoints;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {   
        _timer = (Timer)GetNode("Timer");
        _rnd = new Random();
        Parent = (BaseScript)GetParent();
        Parent.Spawner = this;
        _spawnrate_evaluation_timer = (Timer)GetNode("SpawnrateEvaluationTimer");
        SpawnrateEvaluationTimerWaitTime = _spawnrate_evaluation_timer.WaitTime;
        _spawnrate_evaluation_timer.WaitTime =(SpawnrateEvaluationTimerTimeLeft != 0)? (float)SpawnrateEvaluationTimerTimeLeft : SpawnrateEvaluationTimerWaitTime;
        SpawnPoints = new Spatial[]{(Spatial)Parent.GetNode("SpawnPoint"), (Spatial)Parent.GetNode("SpawnPoint2")};
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public void ChangeWaitTime()
    {
        float satisfactionMultiplicator = (Parent.SatisfactionRating < 34)? 0.67f : (Parent.SatisfactionRating > 65)? 1.33f: 1f;

        if(Parent.CustomerSatisfactionTotal == 0)
            satisfactionMultiplicator = 1f;

        _timer.WaitTime = (60/CustomersPerMinute)*satisfactionMultiplicator;
    }

    private void _on_Timer_timeout()
    {
        if (Parent.Restaurants.Count == 0) return; 

        FoodStall targetFoodStall = TargetFoodStall();
        if (targetFoodStall == null) return;

        Customer customer = CustomerScene.Instance<Customer>();
        customer.TargetRestaurant = targetFoodStall;
        customer.SpawnPoint = SpawnPoints[_rnd.Next(0,2)];
        customer.Transform = customer.SpawnPoint.Transform;
        Parent.AddChild(customer);
    }

    private void _on_SpawnrateEvaluationTimer_timeout()
    {
        if(_spawnrate_evaluation_timer.WaitTime < SpawnrateEvaluationTimerWaitTime)
            _spawnrate_evaluation_timer.WaitTime = SpawnrateEvaluationTimerWaitTime;

        if(Parent.CustomerSatisfactionTotal == 0)
            return;

        if(Parent.SatisfactionRating >= Parent.GoodRatingMin)
            CustomersPerMinute *= 1.02f;

        if(Parent.SatisfactionRating <= Parent.BadRatingMax)
            CustomersPerMinute *= 0.98f;

        ChangeWaitTime();
        Parent.CalculateCustomersPerMinute();
    }

    private FoodStall TargetFoodStall()
    {
        FoodStall targetFoodStall = null;
        int smallestCustomerCount = int.MaxValue;
        int biggesetCustomerCount = 0;

        foreach (FoodStall foodStall in Parent.Restaurants)
        {
            int customerCount = foodStall.IncomingCustomers.Count;
            if(customerCount > biggesetCustomerCount)
                biggesetCustomerCount = customerCount;

            if (customerCount < smallestCustomerCount)
            {
                targetFoodStall = foodStall;
                smallestCustomerCount = customerCount;
            }
        }

        if(smallestCustomerCount == biggesetCustomerCount)
            targetFoodStall = Parent.Restaurants[_rnd.Next(0, Parent.Restaurants.Count)];

        return targetFoodStall;
    }

    public Dictionary<string, object> Save()
	{
		return new Dictionary<string, object>()
		{
			{"Filename", Filename},
			{"Parent", Parent.GetPath()},
			{"PositionX", Transform.origin.x},
			{"PositionY", Transform.origin.y},
			{"PositionZ", Transform.origin.z},
            {"RotationY", 0},
			{"CustomersPerMinute", CustomersPerMinute},
            {"SpawnrateEvaluationTimerTimeLeft", _spawnrate_evaluation_timer.TimeLeft}
		};
	}
}