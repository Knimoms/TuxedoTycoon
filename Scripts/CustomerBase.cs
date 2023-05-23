using Godot;
using System;

public partial class CustomerBase : KinematicBody
{
	public RestaurantBase TargetRestaurant;
	private CourtArea _parent;
	public Vector3 Direction;
	public Vector3 SpawnPoint;

	public Vector3 Velocity = new Vector3();
	public int LineNumber;
	[Export]
	public float EatingTime = 7.0f;

	[Export]
	public float Speed = 5.0f;
	[Export]
	public float Gravity = 9.8f;

	public float JumpVelocity = 4.5f;

	private NavigationAgent _nav_agent;

	private Vector3 _target_window;
	private Timer _timer;
	public bool Waiting = false;
	public bool OrderFinished = false;
	public bool Eating = false;
	private Chair _my_chair;

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = (Timer)GetNode("Timer");
		_nav_agent = (NavigationAgent)GetNode("NavigationAgent");
		this._parent = (CourtArea)this.GetParent();

		//_nav_agent.TargetDesiredDistance = ;
		_target_window = TargetRestaurant.GetNode<Spatial>("OrderWindow").GlobalTransform.origin;
		TargetRestaurant.IncomingCustomers.Add(this);
		LineNumber = TargetRestaurant.IncomingCustomers.Count - 1;
		if (TargetRestaurant.IncomingCustomers.Count > 1)
			this.UpdateTargetLocation(TargetRestaurant.IncomingCustomers[LineNumber - 1].GlobalTransform.origin);
		else
			this.UpdateTargetLocation(_target_window);


	}

	// if((TargetRestaurant.Position - this.Position).Length() > 2) this.Velocity = Direction*(float)delta*Speed;
	// this.MoveAndSlide();
	// GD.Print(this.GetRealVelocity());

	// public override void _Process(float delta)
	// {
	// }

	public override void _PhysicsProcess(float delta)
	{
		this.Rotate(Vector3.Up, (float)(5*delta));

		if (Waiting || Eating)
			return;

		Vector3 velocity = Velocity;

		if (LineNumber != 0)
			UpdateTargetLocation(TargetRestaurant.IncomingCustomers[LineNumber - 1].GlobalTransform.origin);

		Vector3 currentLocation = GlobalTransform.origin;
		Vector3 nextLocation = _nav_agent.GetNextLocation();

		velocity = (nextLocation - currentLocation).Normalized() * Speed;

		if (!IsOnFloor())
			velocity.y -= Gravity;
		// if (OrderFinished)
		//     _nav_agent.SetVelocity(velocity);
		// else 
		// {
			Velocity = velocity;
			MoveAndSlide(Velocity);
		// }

		if ((_nav_agent.GetTargetLocation() - GlobalTransform.origin).Length() <= 0.6f)
			_on_NavigationAgent_target_reached();
	}


	public void QueueUp()
	{
		Waiting = true;
	}

	public void FirstInQueue()
	{
		UpdateTargetLocation(_target_window);
	}

	public void GoToEat()
	{
		_my_chair = _parent.GetRandomFreeChair();
		if(_my_chair == null)
		{
			GetNode<Sprite3D>("Sprite3D").Texture = (Texture)GD.Load("res://Assets/SadEnd.png");
			TargetRestaurant.Refund();
			UpdateTargetLocation(SpawnPoint);
			return;
		}
		GetNode<Sprite3D>("Sprite3D").Texture = (Texture)GD.Load("res://Assets/HappyEnd.png");
		_my_chair.Occupied = true;
		UpdateTargetLocation(_my_chair.GlobalTransform.origin);
	}

	public void UpdateTargetLocation(Vector3 targetLocation)
	{
		_nav_agent.SetTargetLocation(targetLocation);
	}

	public void FinishOrder()
	{
		GoToEat();
		Waiting = false;
		OrderFinished = true;
	}

	public void StartTimer()
	{
		_timer.Start();
	}

	public void StartEating()
	{
		GetNode<Sprite3D>("Sprite3D").Scale *= 0.5f;
		GlobalTransform = new Transform(_my_chair.GlobalTransform.basis, _my_chair.GlobalTransform.origin + Vector3.Up*0.5f);
		Eating = true;
		_timer.WaitTime = EatingTime;
		StartTimer(); 
	}

	public void _on_Timer_timeout()
	{
		if(Eating)
		{
			UpdateTargetLocation(SpawnPoint);
			_my_chair.Occupied = false;
			Eating = false;
			return;
		}

		Waiting = false;
		if (TargetRestaurant.IncomingCustomers.Count - 1 > LineNumber)
			TargetRestaurant.IncomingCustomers[LineNumber + 1].StartTimer();
	}

	private void _on_NavigationAgent_target_reached()
	{
		if (Waiting || Eating)
			return;
		
		if (_nav_agent.GetTargetLocation() == SpawnPoint)
		{
			QueueFree();
			return;
		}
		
		if (_my_chair != null)
		{
			StartEating();
			return;
		}

		if (_nav_agent.GetTargetLocation() != _target_window && (TargetRestaurant.IncomingCustomers.Count == 1 || TargetRestaurant.IncomingCustomers[0] == null || !TargetRestaurant.IncomingCustomers[LineNumber - 1].Waiting))
		{
			UpdateTargetLocation(_target_window);
			return;
		}

		QueueUp();

		if (TargetRestaurant.IncomingCustomers[0] == this)
			TargetRestaurant.Order();
	}

	private void _on_NavigationAgent_velocity_computed(Vector3 safe_velocity)
	{
		Velocity = this.Velocity.MoveToward(safe_velocity, 0.25f);
		MoveAndSlide(Velocity);
	}
}
