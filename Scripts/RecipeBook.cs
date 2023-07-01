using Godot;
using System;
using System.Collections.Generic;

public class RecipeBook : Node
{
    Dictionary<FoodStall, Dish[]> AllRecipes = new Dictionary<FoodStall, Dish[]>();
    
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
