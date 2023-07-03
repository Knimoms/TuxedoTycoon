using Godot;
using System;
using System.Collections.Generic;

public class RecipeBook : Node2D
{

    public int Page;
    public List<FoodStall> FoodStalls;

    Node2D PageL;
    Label FoodStallLabelL;
    Label[] DishesL = new Label[3];
    Dictionary<Label, Sprite[]> IngSpritesL = new Dictionary<Label, Sprite[]>();

    Node2D PageR;
    Label FoodStallLabelR;
    Label[] DishesR = new Label[3];
    Dictionary<Label, Sprite[]> IngSpritesR = new Dictionary<Label, Sprite[]>();

    
    public override void _Ready()
    {
        PageL = (Node2D)GetNode("PageL");
        FoodStallLabelL = (Label)PageL.GetNode("FoodStallLabel");
        for(int i = 0; i < DishesL.Length; i++)
            DishesL[i] = (Label)PageL.GetNode("Dish"+(1+i));

        foreach(Label dish in DishesL)
        {
            IngSpritesL.Add(dish, new Sprite[5]);

            for (int i = 0; i < IngSpritesL[dish].Length; i++)
                IngSpritesL[dish][i] = (Sprite)dish.GetNode("Ing"+(1+i));
        }

        PageR = (Node2D)GetNode("PageR");
        FoodStallLabelR = (Label)PageR.GetNode("FoodStallLabel");
        for(int i = 0; i < DishesR.Length; i++)
            DishesR[i] = (Label)PageR.GetNode("Dish"+(1+i));

        foreach(Label dish in DishesR)
        {
            IngSpritesR.Add(dish, new Sprite[5]);

            for (int i = 0; i < IngSpritesR[dish].Length; i++)
                IngSpritesR[dish][i] = (Sprite)dish.GetNode("Ing"+(1+i));
        }
    }

    public void OpenRecipeBook()
    {
        Page = 1;
        _clear_book();

        for(int i = 0; i < DishesL.Length && FoodStalls.Count != 0; i++)
        {
            DishesL[i].Text = FoodStalls[0].allDishes[i].Name;

            int treshold = IngSpritesL[DishesL[i]].Length;

            for(int j = 0; j < treshold; j++)
            {
                if(FoodStalls[0].allDishes[i].Ings.Count > j && FoodStalls[0].allDishes[i].Unlocked)
                    IngSpritesL[DishesL[i]][j].Texture = Dish.GetIngredientSprite(FoodStalls[0].allDishes[i].Ings[j]);
                else IngSpritesL[DishesL[i]][j].Texture = null;
            }
        }

        for(int i = 0; i < DishesR.Length && FoodStalls.Count > 1; i++)
        {
            DishesR[i].Text = FoodStalls[1].allDishes[i].Name;

            int treshold = IngSpritesR[DishesR[i]].Length;

            for(int j = 0; j < treshold; j++)
            {
                if(FoodStalls[0].allDishes[i].Ings.Count > j && FoodStalls[1].allDishes[i].Unlocked)
                    IngSpritesR[DishesR[i]][j].Texture = Dish.GetIngredientSprite(FoodStalls[0].allDishes[i].Ings[j]);
                else IngSpritesR[DishesR[i]][j].Texture = null;
            }
        }


        Visible = !Visible;
    }

    private void _clear_book()
    {
        for(int i = 0; i < DishesL.Length; i++)
        {

            for(int j = 0; j < IngSpritesL[DishesL[i]].Length; j++)
            {
                IngSpritesL[DishesL[i]][j].Texture = null;
                
            }
        }

        for(int i = 0; i < DishesR.Length; i++)
        {

            for(int j = 0; j < IngSpritesR[DishesR[i]].Length; j++)
            {
                IngSpritesR[DishesR[i]][j].Texture = null;
                
            }
        }
    }


    public Dish[] GetFoodStallDishes(FoodStall foodStall) => foodStall.allDishes;

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
