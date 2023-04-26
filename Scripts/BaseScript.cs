using Godot;
using System;

public partial class BaseScript : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_floor_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx)
	{
		if(event1 is InputEventMouseButton) GetNode<AnimalBase>("AnimalBase").target = postition;
	}
}
