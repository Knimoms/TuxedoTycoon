using Godot;
using System;
using System.Collections.Generic;

public class Minigame2D : Node2D
{
    public List<Ing> ingredientList = new List<Ing>();

    public FoodStall MyFoodStall;
    public Label IngLabel;

    public Sprite[] IngSpots = new Sprite[3];

    public override void _Ready()
    { 
        MyFoodStall = (FoodStall)GetParent();
        IngLabel = (Label)GetNode("IngLabel");
        for(int i = 0; i < IngSpots.Length; i++)
            IngSpots[i] =(Sprite)GetNode("IngSpot" + (i+1));

        IngLabel.Text = PrintRecipe();
    }

    public override void _PhysicsProcess(float delta)
    {
        if(MyFoodStall.OrderedDish != null && IngLabel.Text == "")
            IngLabel.Text = PrintRecipe();
        

        if(MyFoodStall.OrderedDish == null && IngLabel.Text != "")
            IngLabel.Text = "";
    }
    
    public bool CompareLists()
    {
        if(MyFoodStall.OrderedDish == null)
            return false;
    
        if (ingredientList.Count > MyFoodStall.OrderedDish.Ings.Count || ingredientList.Count < MyFoodStall.OrderedDish.Ings.Count)
            return false;
        for (int i = 0; i < ingredientList.Count; i++)
        {
            if (ingredientList[i] != MyFoodStall.OrderedDish.Ings[i])
                return false;

        }
        RecipeCorrect();
        return true;
    }

    public Sprite AddIng(Ing ing)
    {
        if(MyFoodStall.OrderedDish == null)
            return null;
        ingredientList.Add(ing);

        return IngSpots[ingredientList.IndexOf(ing)];
    }

    public void RecipeCorrect()
    {
        MyFoodStall.MiniGameDone();
        _on_TrashButton_pressed();
    }

    private string PrintRecipe()
    {
        string recipe = "";
        foreach(Ing ing in MyFoodStall.OrderedDish.Ings)
        {
            recipe += $"{ing} ";
        }

        return recipe;
    }

    private void _on_CloseButton_pressed()
    {
        MyFoodStall.CloseMiniGame();
        this.QueueFree();
    }

    private void _on_TrashButton_pressed()
    {
		ingredientList.Clear();
        foreach(Sprite fanta in IngSpots)
            fanta.Texture = null;
    }
}
