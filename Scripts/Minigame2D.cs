using Godot;
using System;
using System.Collections.Generic;

public class Minigame2D : Node2D
{
    public List<Ingredient> ingredientList = new List<Ingredient>();
    public FoodStall MyFoodStall;
    public bool Cooking;
    private Sprite _finishedFood;
    private Sprite _wrongFood;
    public Label IngLabel;
    public Sprite[] IngSpots = new Sprite[3];
    private Timer _doneTimer;
    public Sprite Order;
    public FoodSpwn2D foodSpwn2D;
    public Node2D CustomerSprite;


    public override void _Ready()
    {
        MyFoodStall = (FoodStall)GetParent();
        Order = GetNode<Sprite>("Order");
        foodSpwn2D = GetNode<FoodSpwn2D>("FoodSpawner");
        IngLabel = (Label)GetNode("IngLabel");
        for(int i = 0; i < IngSpots.Length; i++)
            IngSpots[i] =(Sprite)GetNode("IngSpot" + (i+1));
        IngLabel.Text = PrintRecipe();
        _finishedFood = GetNode<Sprite>("FinishedFood");
        _wrongFood = GetNode<Sprite>("WrongFood");
        _doneTimer = GetNode<Timer>("DoneTimer");
        CustomerSprite = (Node2D)GetNode("CustomerSprite");
        _doneTimer.WaitTime = 1f;
        _wrongFood.Visible = false;
    }
    public override void _PhysicsProcess(float delta)
    {
        if(MyFoodStall.OrderedDish != null)
            if(MyFoodStall.OrderedDish.DishIcon == null)
            {
                IngLabel.Text = PrintRecipe();
            }
            else
            {
                Order.Texture = MyFoodStall.OrderedDish.DishIcon;
            }
            
        if(MyFoodStall.OrderedDish == null && IngLabel.Text != "" || MyFoodStall.OrderedDish != null && MyFoodStall.OrderedDish.DishIcon != null)
        {
            IngLabel.Text = "";
        }
    }
    private void _on_DoneTimer_timeout()
    {
        CustomerSprite.Visible = false;
        _wrongFood.Visible = false;
        _finishedFood.Texture = null;
        foodSpwn2D.closeButton.Visible = true;
        Cooking = false;
        
    }
    public void CompareLists()
    {
        if(MyFoodStall.OrderedDish == null)
            {
                
                return;
            }

        if(MyFoodStall.OrderedDish.CompareIngredients(ingredientList)){
            _finishedFood.Texture = MyFoodStall.OrderedDish.DishIcon;
            _doneTimer.Start();
            RecipeCorrect();
            
        } else {
            _wrongFood.Visible = true;
            _doneTimer.Start();
            ingredientList.Clear();
            foreach(Sprite fanta in IngSpots)
                fanta.Texture = null;
        }
    }

    public Sprite AddIng(Ingredient ing)
    {
        if(MyFoodStall.OrderedDish == null)
            return null;
        ingredientList.Add(ing);
        return IngSpots[ingredientList.IndexOf(ing)];
    }

    public void RecipeCorrect()
    {
        MyFoodStall.MiniGameDone();
    }

    private string PrintRecipe()
    {
        string recipe = "";
        
        if(MyFoodStall.OrderedDish == null)
            return recipe;

        foreach(Ingredient ing in MyFoodStall.OrderedDish.Ings)
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
        if(Cooking == true)
            return;
		ingredientList.Clear();
        foreach(Sprite fanta in IngSpots)
            fanta.Texture = null;
    }
}
