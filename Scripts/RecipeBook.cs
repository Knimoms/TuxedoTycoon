using Godot;
using System;
using System.Collections.Generic;

public class RecipeBook : Popup
{

    public int Page;
    public List<FoodStall> FoodStalls;
    public BaseScript Parent;

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
        Parent = (BaseScript)GetParent();
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

        Owner = Parent;
    }

    public void OpenRecipeBook()
    {
        _open_recipe_book_page(1);
    }

    private void _open_recipe_book_page(int page)
    {
        this.Popup_();
        if(page < 1)
            return;

        Page = page;
        _clear_book();

        for(int i = 0; i < DishesL.Length && FoodStalls.Count >= page*2-1; i++)
        {
            DishesL[i].Text = FoodStalls[page*2-2].allDishes[i].Name;

            int treshold = IngSpritesL[DishesL[i]].Length;

            for(int j = 0; j < treshold; j++)
            {
                if(FoodStalls[page*2-2].allDishes[i].Ings.Count > j && FoodStalls[page*2-2].allDishes[i].Unlocked)
                    IngSpritesL[DishesL[i]][j].Texture = Dish.GetIngredientSprite(FoodStalls[page*2-2].allDishes[i].Ings[j]);
                else IngSpritesL[DishesL[i]][j].Texture = null;
            }
        }

        for(int i = 0; i < DishesR.Length && FoodStalls.Count >= page*2; i++)
        {
            DishesR[i].Text = FoodStalls[page*2-1].allDishes[i].Name;

            int treshold = IngSpritesR[DishesR[i]].Length;

            for(int j = 0; j < treshold; j++)
            {
                if(FoodStalls[page*2-1].allDishes[i].Ings.Count > j && FoodStalls[page*2-1].allDishes[i].Unlocked)
                    IngSpritesR[DishesR[i]][j].Texture = Dish.GetIngredientSprite(FoodStalls[page*2-1].allDishes[i].Ings[j]);
                else IngSpritesR[DishesR[i]][j].Texture = null;
            }
        }

        GD.Print(Page);
    }

    private void _on_ButtonR_pressed()
    {
        _open_recipe_book_page(Page+1);
    }

    private void _on_ButtonL_pressed()
    {
        _open_recipe_book_page(Page-1);
    }

    private void _clear_book()
    {
        for(int i = 0; i < DishesL.Length; i++)
        {
            DishesL[i].Text = "";

            for(int j = 0; j < IngSpritesL[DishesL[i]].Length; j++)
                IngSpritesL[DishesL[i]][j].Texture = null;
        }

        for(int i = 0; i < DishesR.Length; i++)
        {
            DishesR[i].Text = "";

            for(int j = 0; j < IngSpritesR[DishesR[i]].Length; j++)
                IngSpritesR[DishesR[i]][j].Texture = null;
        }
    }


    public Dish[] GetFoodStallDishes(FoodStall foodStall) => foodStall.allDishes;

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
