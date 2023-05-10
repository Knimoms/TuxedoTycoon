using Godot;
using System;

public partial class CustomerBase : CharacterBody3D
{
	public RestaurantBase TargetRestaurant;
	private CourtArea _parent;
	public Vector3 Direction;

	public int LineNumber;

	[Export]
	public const float Speed = 5.0f;
	[Export]
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public const float JumpVelocity = 4.5f;

	private NavigationAgent3D _nav_agent;

	private Vector3 _target_window;


	public bool Waiting = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

		// Direction = (TargetRestaurant.Position - this.Position).Normalized();
		Vector3 velocity = Velocity;
		if(Waiting) 
			return;

		if(LineNumber != 0)
			this.UpdateTargetLocation(this.TargetRestaurant.IncomingCustomers[LineNumber-1].GlobalPosition);
		Vector3 currentLocation = this.GlobalTransform.Origin;
		Vector3 nextLocation = _nav_agent.GetNextPathPosition();

		velocity = (nextLocation - currentLocation).Normalized() * Speed;
		if (!IsOnFloor())
			velocity.Y -= Gravity;
		this.Velocity = velocity;
		MoveAndSlide();
		if((_nav_agent.TargetPosition -this.GlobalPosition).Length() <= 0.8f) 
			_on_navigation_agent_3d_target_reached();

	}


	public void QueueUp()
	{
		this.Waiting = true;
	}

	public void FirstInQueue()
	{
		this.UpdateTargetLocation(_target_window);
	}

	public void UpdateTargetLocation(Vector3 targetLocation)
	{
		_nav_agent.TargetPosition = targetLocation;
	}

	public void FinishOrder()
	{
		//this.UpdateTargetLocation(this.GetParent().GetNode<CustomerSpawner>("CustomerSpawner").Position - Vector3.Forward*4);
		//Waiting = false;

		this.QueueFree();
	}

	private void _on_navigation_agent_3d_target_reached()
	{		
		if(_nav_agent.TargetPosition != _target_window && (this.TargetRestaurant.IncomingCustomers.Count == 1 || this.TargetRestaurant.IncomingCustomers[0] == null || !this.TargetRestaurant.IncomingCustomers[LineNumber-1].Waiting))
		{
			UpdateTargetLocation(_target_window);
			return;
		}
			
		this.QueueUp();
		if(this.TargetRestaurant.IncomingCustomers[0] == this)
			TargetRestaurant.Order();
	}
}
