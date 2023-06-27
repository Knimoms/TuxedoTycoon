using Godot;
using System.Collections.Generic;
using System;

public class Dish : Spatial
{
    public string Name {get; private set;}
    
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
    
    public List<Ing> Ings = new List<Ing>();

    public override void _Ready()
    {
        string[] splittedFilename = Filename.Split(new char[]{'/', '.'});
        Name = splittedFilename[splittedFilename.Length-2];

        Ing[] _allIngs = new Ing[] {Ing1, Ing2, Ing3, Ing4, Ing5};

        foreach(Ing ing in _allIngs)
        {
            if(ing != Ing.Null) 
                Ings.Add(ing);
        }
    }
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
