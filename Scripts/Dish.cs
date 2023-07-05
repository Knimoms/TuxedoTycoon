using Godot;
using System.Collections.Generic;
using System;

public class Dish : Spatial
{
    public string DishName {get; private set;}

    private bool _unlocked = false;
    public bool Unlocked => _unlocked;
    
    [Export]
    public Ingredient Ing1 = Ingredient.Null;
    [Export]
    public Ingredient Ing2 = Ingredient.Null;
    [Export]
    public Ingredient Ing3 = Ingredient.Null;
    [Export]
    public Ingredient Ing4 = Ingredient.Null;
    [Export]
    public Ingredient Ing5 = Ingredient.Null;

    [Export]
    public float MealPriceValue;
    [Export]
    public string MealPriceMagnitude;

    private Tuxdollar _mealprice;
    public Tuxdollar MealPrice
    {
        get{ return _mealprice;}
        set {_mealprice = (value > Tuxdollar.ZeroTux)? value : Tuxdollar.ZeroTux;}
    }
    
    public List<Ingredient> Ings = new List<Ingredient>();

    public static Dictionary<Ingredient, Texture> IngredientSprites;

    public override void _Ready()
    {
        Owner = GetParent();
        IngredientSprites = new Dictionary<Ingredient, Texture>();

        string[] splittedFilename = Filename.Split(new char[]{'/', '.'});
        DishName = splittedFilename[splittedFilename.Length-2];

        Ingredient[] _allIngs = new Ingredient[] {Ing1, Ing2, Ing3, Ing4, Ing5};

        foreach(Ingredient ing in _allIngs)
        {
            if(ing != Ingredient.Null) 
                Ings.Add(ing);
        }
    }

    public bool CompareIngredients(List<Ingredient> ingredients)
    {
        if(ingredients.Count != Ings.Count)
            return false;

        for(int i = 0; i < Ings.Count; i++)
        {
            if(ingredients[i] != Ings[i])
                return false;
        }

        _unlocked = true;
        return true;
    }

    public static Texture GetIngredientSprite(Ingredient ingredient)
    {
        if(!IngredientSprites.ContainsKey(ingredient))
            IngredientSprites.Add(ingredient, (Texture)GD.Load($"res://Assets/MinigameStuff/Ingredients/{ingredient}.png"));

        return IngredientSprites[ingredient];
    }
}


public enum Ingredient{
    Null,
    Ice,
    Milk,
    Fish,
    Seaweed,
    Rice,
    CookedFish,
    Lemon,
    Potato,
    Garnish,
    Shrip,
    Veggies,
    RicePaper,
    Bread,
    Meat,
    Salad,
    Broth,
    Sauce,
    Noodles
}
