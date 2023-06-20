using Godot;
using System.Collections.Generic;
using System;

public class Dish : Spatial
{
    [Export]
    public Ing Ing1 = Ing.Null;
    [Export]
    public Ing Ing2 = Ing.Null;
    [Export]
    public Ing Ing3 = Ing.Null;
    [Export]
    public Ing Ing4 = Ing.Null;
    [Export]
    public Ing Ing5 = Ing.Null;
    
    private Ing[] _allIngs;

    [Export]
    public float MealPriceValue;

    [Export]
    public string MealPriceMagnitude;

    private Tuxdollar _mealprice;
    public Tuxdollar MealPrice
    {
        get{ return _mealprice;}
    }
    public List<Ing> Ings = new List<Ing>();

    public Dish()
    {
        _mealprice = new Tuxdollar(MealPriceValue, MealPriceMagnitude);

        _allIngs = new Ing[] {Ing1, Ing2, Ing3, Ing4, Ing5};

        foreach(Ing ing in _allIngs)
        {
            if(ing != Ing.Null) 
                Ings.Add(ing);
        }
    }

    public override void _Ready()
    {
        _mealprice = new Tuxdollar(MealPriceValue, MealPriceMagnitude);

        _allIngs = new Ing[] {Ing1, Ing2, Ing3, Ing4, Ing5};

        foreach(Ing ing in _allIngs)
        {
            if(ing != Ing.Null) 
                Ings.Add(ing);
        }
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

public enum Ing{
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
