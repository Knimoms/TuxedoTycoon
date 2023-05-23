using Godot;
using System;

public partial class Camera : Spatial
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		float horizontal = Input.GetAxis("left", "right");
		float vertical = Input.GetAxis("up", "down");
		this.Translation += new Vector3(horizontal, 0, vertical)*(float)delta*10;
	}
}
