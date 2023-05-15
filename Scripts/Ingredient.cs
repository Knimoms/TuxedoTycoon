using Godot;
using System;
using System.Collections.Generic;



public partial class Ingredient : Node3D
{
    
    [Export]
    public string IngName { get; set; }

    public KitchenCounter kitchenCounter;

    public override void _Ready()
	{
		kitchenCounter = GetParent<KitchenCounter>();
	}

    
    private void _on_static_body_3d_input_event(Node camera, InputEvent event1, Vector3 postition, Vector3 normal, int shape_idx) 
	{
		if(Input.IsActionJustPressed("place"))
        {
            AddInUserlist(this);
            GD.Print("Pressed!");
            foreach(Ingredient a in kitchenCounter.minigame.userList ){
                GD.Print(a.IngName);
            }
        }
	}

    public void AddInUserlist (Ingredient ing)
    {
        kitchenCounter.minigame.userList.Add(ing);
    }

}
