using Godot;
using System;

public partial class Customer : KinematicBody
{

	
	private Spatial _my_body;
	public FoodStall TargetRestaurant;
	private CourtArea _parent;
	public Vector3 Direction;
	public Vector3 SpawnPoint;
	private NavigationAgent _nav_agent;
	private Vector3 _target_window;
	private Timer _timer;
	private Timer _patienceTimer;
	private Chair _my_chair;
	private Sprite3D _my_sprite;

	[Export]
	public float EatingTime = 7.0f;
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float Gravity = 9.8f;

	public Vector3 Velocity = new Vector3();
	public int LineNumber;
	public CustomerState State;
	public bool OrderFinished = false;

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_my_body = (Spatial)GetNode("Body");
		_my_sprite = (Sprite3D)GetNode("Sprite3D");
		_timer = (Timer)GetNode("Timer");
		_patienceTimer = (Timer)GetNode("PatienceTimer");
		_patienceTimer.Start();
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
		
		State = CustomerState.Walking;
	}

	public override void _PhysicsProcess(float delta)
	{
		_my_sprite.Rotate(Vector3.Up, (float)(5*delta));

		if (State == CustomerState.Waiting || State == CustomerState.Eating)
			return;

		Vector3 velocity = Velocity;

		if (LineNumber != 0 && !OrderFinished)
			UpdateTargetLocation(TargetRestaurant.IncomingCustomers[LineNumber - 1].GlobalTransform.origin);

		Vector3 currentLocation = GlobalTransform.origin;
		Vector3 nextLocation = _nav_agent.GetNextLocation();

		velocity = (nextLocation - currentLocation).Normalized() * Speed;

		if (!IsOnFloor())
			velocity.y -= Gravity;
		
		Velocity = velocity;
		MoveAndSlide(Velocity);

		_my_body.LookAt(_nav_agent.GetNextLocation(), Vector3.Up);

		if ((_nav_agent.GetTargetLocation() - GlobalTransform.origin).Length() <= 0.6f)
			_on_NavigationAgent_target_reached();
	}

	public void UpdateTargetLocation(Vector3 targetLocation)
	{
		_nav_agent.SetTargetLocation(targetLocation);
	}


	public void QueueUp()
	{
		State = CustomerState.Waiting;
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
			_my_sprite.Texture = (Texture)GD.Load("res://Assets/SadEnd.png");
			TargetRestaurant.Refund();
			UpdateTargetLocation(SpawnPoint);
			return;
		}
		_my_sprite.Texture = (Texture)GD.Load("res://Assets/HappyEnd.png");
		_my_chair.Occupied = true;
		UpdateTargetLocation(_my_chair.GlobalTransform.origin);
	}

	public void FinishOrder()
	{
		GoToEat();
		State = CustomerState.Walking;
		OrderFinished = true;
	}

	public void StartEating()
	{
		_my_sprite.Scale *= 0.5f;
		GlobalTransform = new Transform(_my_chair.GlobalTransform.basis, _my_chair.GlobalTransform.origin + Vector3.Up*0.5f);
		_my_body.Rotation = _my_chair.Rotation;
		State = CustomerState.Eating;
		_timer.WaitTime = EatingTime;
		StartTimer(); 
	}

	public void TakeAwayFood()
	{
		State = CustomerState.Walking;
		OrderFinished = true;
		LineNumber = 0;
		_my_sprite.Texture = (Texture)GD.Load("res://Assets/HappyEnd.png");
		TargetRestaurant.IncomingCustomers.Remove(this);		
		UpdateTargetLocation(SpawnPoint);
	}

	public void StartTimer()
	{
		_timer.Start();
	}

	public void _on_PatienceTimer_timeout()
	{
		if(LineNumber > 15) 
		{
			UpdateTargetLocation(SpawnPoint);
			_my_sprite.Texture = (Texture)GD.Load("res://Assets/SadEnd.png");
			TargetRestaurant.IncomingCustomers.Remove(this);
			for(int i = LineNumber; i < TargetRestaurant.IncomingCustomers.Count; i++)
			{	
				TargetRestaurant.IncomingCustomers[i].LineNumber--;
				TargetRestaurant.IncomingCustomers[i].State = CustomerState.Walking;
			}
			LineNumber = 0;
			State = CustomerState.Walking;;
		}
	}

	public void _on_Timer_timeout()
	{
		if(State == CustomerState.Eating)
		{
			UpdateTargetLocation(SpawnPoint);
			_my_chair.Occupied = false;
			State = CustomerState.Walking;
			return;
		}

		State = CustomerState.Walking;
		if (TargetRestaurant.IncomingCustomers.Count - 1 > LineNumber)
			TargetRestaurant.IncomingCustomers[LineNumber + 1].StartTimer();
	}

	private void _on_NavigationAgent_target_reached()
	{
		if (State == CustomerState.Waiting || State == CustomerState.Eating)
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

		if (_nav_agent.GetTargetLocation() != _target_window && (TargetRestaurant.IncomingCustomers.Count == 1 || TargetRestaurant.IncomingCustomers[0] == null || TargetRestaurant.IncomingCustomers[LineNumber - 1].State != CustomerState.Waiting))
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

public enum CustomerState
{
	Waiting,
	Walking,
	Eating

}
