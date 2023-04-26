using Godot;
using System;

public partial class AnimalBase : Node3D
{		
	public Vector3 target;

	[Export]
	int speed = 10;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		target = this.Position;
		GD.Print(this.Position);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		if(this.Position.DistanceTo(target) > 0.5f)this.Position += (target - this.Position).Normalized()*(float)delta*speed;
		
	}

	private void _on_character_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{

	}
}
