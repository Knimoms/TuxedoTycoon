using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Minigame : Spatial
{
    //private string _compare1;
    //private string _compare2;

    public FoodStall MyFoodStall;
    public List<Ingredient> userList = new List<Ingredient>
    {
    };

    public List<IngredientBase> ingredientList = new List<IngredientBase>
    {
    };

    public bool Done = false;

    Ingredient[] recipe1 = new Ingredient[]
    {
        new Ingredient { IngName = "Ing1" },
        new Ingredient { IngName = "Ing2" },
        new Ingredient { IngName = "Ing3" },
    };

    private void _on_CloseButton_pressed()
    {
        MyFoodStall._parent.Parent.BaseCam.MakeCurrent();
        MyFoodStall.ToggleVisibility();
        if(MyFoodStall.IncomingCustomers.Count > 0 && MyFoodStall.IncomingCustomers[0].Waiting == true)
        {
            MyFoodStall.TimerProp.Start();
        }
        this.QueueFree();
    }

    public bool CompareLists()
    {
        GD.Print("UL:");
		foreach (Ingredient a in userList)
        {
            GD.Print(a.IngName);
        }
		GD.Print("RL:");
        foreach (Ingredient b in recipe1)
        {
            GD.Print(b.IngName);
        }
        if (userList.Count > recipe1.Length || userList.Count < recipe1.Length)
            return false;
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].IngName != recipe1[i].IngName)
                return false;

        }
        return true;
    }

}
