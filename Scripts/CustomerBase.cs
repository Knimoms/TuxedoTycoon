using Godot;
using System;

public partial class CustomerBase : CharacterBody3D
{
	public RestaurantBase TargetRestaurant;
	private BaseScript _parent;
	public Vector3 Direction;

	[Export]
	public const float Speed = 5.0f;
	[Export]
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public const float JumpVelocity = 4.5f;


	private bool _entered = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._parent = (BaseScript)this.GetParent();
	}

		// if((TargetRestaurant.Position - this.Position).Length() > 2) this.Velocity = Direction*(float)delta*Speed;
		// this.MoveAndSlide();
		// GD.Print(this.GetRealVelocity());

	public override void _Process(double delta)
	{

		Direction = (TargetRestaurant.Position - this.Position).Normalized();
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= Gravity * (float)delta;

		// Handle Jump.
			

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		if(_entered)
		{
			return;
		}
		if ((this.TargetRestaurant.Position - this.Position).Length() > 2)
		{
			velocity.X = Direction.X * Speed;
			velocity.Z = Direction.Z * Speed;
		}
		else
		{
			_entered = true;
			EnterRestaurant();
			velocity.X = 0;
			velocity.Z = 0;
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}

	public void EnterRestaurant()
	{
		this.TargetRestaurant.Order(this);
		
	}

	public void LeaveRestaurant()
	{
		this.QueueFree();
	}
}
