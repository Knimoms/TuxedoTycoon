using Godot;
using System;
using System.Collections.Generic;

public class Minigame2D : Node2D
{
    public FoodStall MyFoodStall = new FoodStall();
    public FoodStall foodStall;
    public List<Ing> ingredientList = new List<Ing>
    {
        
    };

    public override void _Ready()
    {
        foodStall = GetParent<FoodStall>();
    
    }

    public bool CompareLists()
    {
        GD.Print("UL:");
		foreach (Ing a in ingredientList)
        {
            GD.Print(a);
            
        }
		GD.Print("RL:");
        foreach (Ing b in MyFoodStall.OrderedDish.Ings)
        {
            GD.Print(b);
        }
        if (ingredientList.Count > MyFoodStall.OrderedDish.Ings.Count || ingredientList.Count < MyFoodStall.OrderedDish.Ings.Count)
            return false;
        for (int i = 0; i < ingredientList.Count; i++)
        {
            if (ingredientList[i] != MyFoodStall.OrderedDish.Ings[i])
                return false;

        }
        return true;
    }

}
