using Godot;
using System;

public partial class CustomerBase : CharacterBody3D
{
	public RestaurantBase TargetRestaurant;
	private CourtArea _parent;
	public Vector3 Direction;

	public Vector3 SpawnPoint;

	public int LineNumber;

	[Export]
	public const float Speed = 5.0f;
	[Export]
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public const float JumpVelocity = 4.5f;

	private NavigationAgent3D _nav_agent;

	private Vector3 _target_window;

	private Timer _timer;
	public bool Waiting = false;
	public bool OrderFinished = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = (Timer)GetNode("Timer");
		_nav_agent = (NavigationAgent3D)GetNode("NavigationAgent3D");
		this._parent = (CourtArea)this.GetParent();

		//_nav_agent.TargetDesiredDistance = ;
		_target_window = TargetRestaurant.GetNode<Node3D>("OrderWindows").GlobalPosition;
		TargetRestaurant.IncomingCustomers.Add(this);
		LineNumber = TargetRestaurant.IncomingCustomers.Count-1;
		if(TargetRestaurant.IncomingCustomers.Count > 1)
			this.UpdateTargetLocation(TargetRestaurant.IncomingCustomers[LineNumber-1].GlobalPosition);
		else 		
			this.UpdateTargetLocation(_target_window);
		

	}

		// if((TargetRestaurant.Position - this.Position).Length() > 2) this.Velocity = Direction*(float)delta*Speed;
		// this.MoveAndSlide();
		// GD.Print(this.GetRealVelocity());

	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta) 
	{
		if(Waiting) 
			return;

		Vector3 velocity = Velocity;

		if(LineNumber != 0)
			UpdateTargetLocation(TargetRestaurant.IncomingCustomers[LineNumber-1].GlobalPosition);

		Vector3 currentLocation = GlobalTransform.Origin;
		Vector3 nextLocation = _nav_agent.GetNextPathPosition();

		velocity = (nextLocation - currentLocation).Normalized() * Speed;
		if (!IsOnFloor())
			velocity.Y -= Gravity;
		if(OrderFinished)
			_nav_agent.SetVelocity(velocity);
		else	
		{
			Velocity = velocity;
			MoveAndSlide();
		}
		
		if((_nav_agent.TargetPosition -GlobalPosition).Length() <= 0.6f) 
			_on_navigation_agent_3d_target_reached();
	}


	public void QueueUp()
	{
		Waiting = true;
	}

	public void FirstInQueue()
	{
		UpdateTargetLocation(_target_window);
	}

	public void UpdateTargetLocation(Vector3 targetLocation)
	{
		_nav_agent.TargetPosition = targetLocation;
	}

	public void FinishOrder()
	{
		UpdateTargetLocation(SpawnPoint);
		Waiting = false;
		OrderFinished = true;
	}

	public void StartTimer()
	{
		_timer.Start();
	}

	public void _on_timer_timeout()
	{
		Waiting = false;
		if(TargetRestaurant.IncomingCustomers.Count-1 > LineNumber )
			TargetRestaurant.IncomingCustomers[LineNumber+1].StartTimer();
	}

	private void _on_navigation_agent_3d_target_reached()
	{		
		if(Waiting)
			return;

		if(OrderFinished)
		{
			QueueFree();
			return;
		}
		
		if(_nav_agent.TargetPosition != _target_window && (TargetRestaurant.IncomingCustomers.Count == 1 || TargetRestaurant.IncomingCustomers[0] == null || !TargetRestaurant.IncomingCustomers[LineNumber-1].Waiting))
		{
			UpdateTargetLocation(_target_window);
			return;
		}
			
		QueueUp();

		if(TargetRestaurant.IncomingCustomers[0] == this)
			TargetRestaurant.Order();
	}

	private void _on_navigation_agent_3d_velocity_computed(Vector3 safe_velocity)
	{
		Velocity = this.Velocity.MoveToward(safe_velocity, 0.25f);
		MoveAndSlide();
	}
}
