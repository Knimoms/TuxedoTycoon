using Godot;
using System;

public partial class Customer : KinematicBody
{

	[Export]
	public float EatingTime = 7.0f;
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float Gravity = 9.8f;
	public int LineNumber;
	public float QueueTime;

	private int _satisfaction = 50;
	public int Satisfaction 
	{
		get => _satisfaction;
		set{_satisfaction = (value >= 0 && value <= 100)? value : (value < 0)? 0 : 100;}
	}

	public Vector3 Velocity = new Vector3();
	private CustomerState _state;
	public CustomerState State{
		get => _state;
		set
		{
			_state = value;
			if(_state.ToString().Find("Walking") != -1)
				_animation_player.Play("penguinWalkShort -loop");
			else _animation_player.Play("idle");
		}
	}
	public bool OrderFinished = false;
	private Dish OrderedDish;

	private Spatial _my_body;
	public FoodStall TargetRestaurant;
	public static BaseScript Parent;
	public Vector3 Direction;
	public Spatial SpawnPoint;
	private NavigationAgent _nav_agent;
	private Vector3 _target_window;
	private Timer _timer;
	private Timer _patienceTimer;
	private Chair _my_chair;
	private Sprite3D _my_sprite;
	private static BaseScript _base_script;
	private AnimationPlayer _animation_player;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(_base_script == null)
			_base_script = (BaseScript)GetViewport().GetNode("Spatial");
		_my_body = (Spatial)GetNode("Body");
		_my_sprite = (Sprite3D)GetNode("Sprite3D");
		_timer = (Timer)GetNode("Timer");
		_timer.Stop();
		_patienceTimer = (Timer)GetNode("PatienceTimer");
		_patienceTimer.Start();
		_nav_agent = (NavigationAgent)GetNode("NavigationAgent");
		_animation_player = (AnimationPlayer)_my_body.GetNode("penguin/AnimationPlayer");

		if(Parent == null)
			Parent = (BaseScript)this.GetParent();

		_target_window = TargetRestaurant.OrderWindow.GlobalTransform.origin;
		TargetRestaurant.IncomingCustomers.Add(this);
		LineNumber = TargetRestaurant.IncomingCustomers.Count - 1;
		
		Spatial stopover = (Spatial)TargetRestaurant.GetNodeOrNull("Stopover");
		if(stopover != null)
		{
			UpdateTargetLocation(stopover.GlobalTransform.origin);
			State = CustomerState.WalkingToStopover;
		}
		else {
			UpdateTargetLocation(TargetRestaurant.GetEntryQueueSpot(LineNumber));
			State = CustomerState.WalkingToQueue;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if(State < CustomerState.WalkingToTable) 
			QueueTime += delta;

		_my_sprite.Rotate(Vector3.Up, (float)(5*delta));

		if (State == CustomerState.WaitingInQueue || State == CustomerState.EatingFood)
			return;

		Vector3 velocity = Velocity;

		if (LineNumber != 0 && !OrderFinished && State != CustomerState.WalkingToQueue && State != CustomerState.WalkingToStopover)
			UpdateTargetLocation(TargetRestaurant.IncomingCustomers[LineNumber - 1].GlobalTransform.origin);

		Vector3 currentLocation = GlobalTransform.origin;
		Vector3 nextLocation = _nav_agent.GetNextLocation();

		velocity = (nextLocation - currentLocation).Normalized() * Speed;

		if (!IsOnFloor())
			velocity.y -= Gravity;
		
		Velocity = velocity;
		MoveAndSlide(Velocity);

		_my_body.LookAt(_nav_agent.GetNextLocation(), Vector3.Up);
		_my_body.RotationDegrees = new Vector3(0, _my_body.RotationDegrees.y, 0);
	}

	public void UpdateTargetLocation(Vector3 targetLocation)
	{
		_nav_agent.SetTargetLocation(targetLocation);
	}


	public void QueueUp()
	{
		State = CustomerState.WaitingInQueue;			
	}

	public void FirstInQueue()
	{
		UpdateTargetLocation(_target_window);
	}

	public void QueueTimeSatisfaction()
	{
		QueueTime -= 51+TargetRestaurant.TimerProp.WaitTime;
		Satisfaction -= (int)QueueTime;
	}

	public void GoToEat()
	{
		Parent.TransferMoney(OrderedDish.MealPrice*TargetRestaurant.Multiplicator);
		State = CustomerState.WalkingToTable;
		QueueTimeSatisfaction();
		_my_chair = Parent.GetRandomFreeChair();
		if(_my_chair == null || !OrderedDish.Unlocked)
		{
			_my_sprite.Texture = (Texture)GD.Load("res://Assets/SadEnd.png");
			TargetRestaurant.Refund(OrderedDish);
			TargetRestaurant.OrderedDish = null;
			Satisfaction -= 70;
			Leave();
			return;
		}
		_nav_agent.NavigationLayers = 2;
		TargetRestaurant.OrderedDish = null;
		State = CustomerState.WalkingToTable;
		_my_sprite.Texture = (Texture)GD.Load("res://Assets/HappyEnd.png");
		_my_chair.Occupied = true;
		UpdateTargetLocation(_my_chair.GlobalTransform.origin);
	}

	public void FinishOrder()
	{
		TargetRestaurant.IncomingCustomers.Remove(this);
		GoToEat();
		OrderFinished = true;
	}

	public void StartEating()
	{
		_my_sprite.Scale *= 0.5f;
		GlobalTransform = new Transform(_my_chair.GlobalTransform.basis, _my_chair.GlobalTransform.origin);
		_my_body.GlobalRotation = _my_chair.GlobalRotation;
		State = CustomerState.EatingFood;
		_timer.WaitTime = EatingTime;
		StartTimer(); 
	}

	public void TakeAwayFood()
	{
		Satisfaction = 70 + (int)(_base_script.SatisfactionBonus/3.0f);
		OrderFinished = true;
		LineNumber = 0;
		_my_sprite.Texture = (Texture)GD.Load("res://Assets/HappyEnd.png");
		TargetRestaurant.IncomingCustomers.Remove(this);	
		Leave();	
	}

	public void Leave()
	{
		float tip = Math.Max(0,((float)Satisfaction/50)-1);
		if(OrderedDish != null)
			Parent.TransferMoney(tip*OrderedDish.MealPrice*TargetRestaurant.Multiplicator);

		UpdateTargetLocation(SpawnPoint.GlobalTransform.origin);
		State = CustomerState.WalkingToExit;
		Parent.AddSatisfaction(Satisfaction);
	}

	public void StartTimer()
	{
		_timer.Start();
	}

	public void _on_PatienceTimer_timeout()
	{
		if(LineNumber > 10) 
		{
			Satisfaction = 25;
			Leave();
			_my_sprite.Texture = (Texture)GD.Load("res://Assets/SadEnd.png");
			TargetRestaurant.IncomingCustomers.Remove(this);
			for(int i = LineNumber; i < TargetRestaurant.IncomingCustomers.Count; i++)
			{	
				TargetRestaurant.IncomingCustomers[i].LineNumber--;
				TargetRestaurant.IncomingCustomers[i].State = CustomerState.WalkingInQueue;
			}
			LineNumber = 0;
		}
	}

	public void _on_Timer_timeout()
	{
		if(State == CustomerState.EatingFood)
		{
			Leave();
			_my_chair.Occupied = false;
			return;
		}

		State = CustomerState.WalkingInQueue;

		if (TargetRestaurant.IncomingCustomers.Count - 1 > LineNumber && TargetRestaurant.IncomingCustomers[LineNumber + 1].State == CustomerState.WaitingInQueue)
			TargetRestaurant.IncomingCustomers[LineNumber + 1].StartTimer();
	}

	private void _on_NavigationAgent_target_reached()
	{
		if(State == CustomerState.EatingFood || State == CustomerState.WaitingInQueue)
			return;		

		switch(State)
		{
			case CustomerState.WalkingToExit:
				QueueFree();
				break;
			case CustomerState.WalkingToTable:
				StartEating();
				break;
			case CustomerState.WalkingToQueue:
				if (LineNumber > 0)
					this.UpdateTargetLocation(TargetRestaurant.IncomingCustomers[LineNumber - 1].GlobalTransform.origin);
				else
					this.UpdateTargetLocation(_target_window);

				State = CustomerState.WalkingToQueueWaitSpot;
				_nav_agent.TargetDesiredDistance = 0.6f;
				break;
			case CustomerState.WalkingToStopover:
				UpdateTargetLocation(TargetRestaurant.GetEntryQueueSpot(LineNumber));
				State = CustomerState.WalkingToQueue;
				break;

			default:
				QueueUp();

				if (LineNumber == 0)
					OrderedDish = TargetRestaurant.Order();
				break;
		}
	}
}

public enum CustomerState
{
	WalkingInQueue,
	WaitingInQueue,
	WalkingToTable,
	EatingFood,
	WalkingToExit,
	WalkingToQueueWaitSpot,
	WalkingToStopover,
	WalkingToQueue

}
