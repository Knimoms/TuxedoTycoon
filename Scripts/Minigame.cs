using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Minigame : Node
{
    private string _compare1;
    private string _compare2;
    public List<Ingredient> userList = new List<Ingredient>
    {
    };
    
    
    Ingredient[] recipe1 = new Ingredient[]
    {
        new Ingredient { IngName = "Ing1" },
        new Ingredient { IngName = "Ing2" },
        new Ingredient { IngName = "Ing3" },
    };

    public bool CompareLists()
    {
        if(userList.Count > recipe1.Length)
            return false;
        for(int i = 0; i < userList.Count; i++)
        {
            if(userList[i].IngName != recipe1[i].IngName)
                return false;
            
        }
        return true;
    }
    
}
