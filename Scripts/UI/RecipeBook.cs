using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;


public class RecipeBook : Control
{
    [Export]
    public PackedScene RecipeBookSlotScene;
    
    public List<FoodStall> FoodStalls;
    public BaseScript Parent;
    private Dish _current_dish;
    public Dish CurrentDish
    {
        get => _current_dish;
        set{
            _current_dish = value;
            if(_current_dish == null)
                return;
            RecipeSprite.Texture = _current_dish.RecipeTexture;
            DifficultyWindows.RectClipContent = !_current_dish.Unlocked;
        }
    }
    private HBoxContainer _hbox_container;
    public Control DifficultyWindows;
    public Sprite RecipeSprite;
    
    public override void _Ready()
    {
        RecipeBookSlot.RecipeBook = this;
        Parent = (BaseScript)GetParent();
        _hbox_container = (HBoxContainer)GetNode("Sprite/ScrollContainer/HBoxContainer");
        RecipeSprite = (Sprite)GetNode("Sprite/DifficultyWindow/RecipeSprite");
        DifficultyWindows = (Control)GetNode("Sprite/DifficultyWindow");
        Visible = false;
    }

    public void AddDishesToBook()
    {
        foreach(FoodStall foodStall in FoodStalls)
            foreach(Dish dish in foodStall.allDishes)
                AddDishToBook(dish);
    }

    public void AddDishToBook(Dish dish)
    {
        RecipeBookSlot recipeBookSlot = (RecipeBookSlot)RecipeBookSlotScene.Instance();
        recipeBookSlot.Dish = dish;
        _hbox_container.AddChild(recipeBookSlot);
        _hbox_container.RectMinSize = _hbox_container.RectMinSize + Vector2.Right*100;
    }

    private void _on_RecipeBook_visibility_changed()
    {
        CurrentDish = _current_dish;
        Parent.IState = Visible? InputState.RecipeBookOpened : InputState.Default;
    }

    public Dish[] GetFoodStallDishes(FoodStall foodStall) => foodStall.allDishes;

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
