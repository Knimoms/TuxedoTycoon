using Godot;
using System;

public partial class KitchenCounter : Node3D
{
	public Minigame minigame;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		minigame = GetParent<Minigame>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
