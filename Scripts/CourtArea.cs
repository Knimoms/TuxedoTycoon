using Godot;
using System;
using System.Collections.Generic;

public partial class CourtArea : Spatial
{
	public BaseScript Parent;
	public List<Chair> Chairs = new List<Chair>();

	public Vector3 SpawnPoint {get; private set;}

	Random rnd = new Random();
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		GD.Print(this);
		SpawnPoint = GetNode<Spatial>("SpawnPoint").Transform.origin;

		Parent = (BaseScript)this.GetParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{	
	}

	public Chair GetRandomFreeChair()
	{
		if(Chairs.Count == 0) return null;
		int i = rnd.Next(0, Chairs.Count), j = i;
		Chair chair = Chairs[i];
		while(chair.Occupied)
		{
			if(i >= Chairs.Count-1) 
				i = -1;
			chair = Chairs[++i];
			if(j == i) return null;
		}

		return chair;
	}
}
