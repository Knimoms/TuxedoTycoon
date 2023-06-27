using Godot;
using System;

public class AdvertisingManager : PopupMenu
{
    public int NewspaperAdsLvl = 0;
    public int OnlineAdsLvl = 0;
    public int BillboardAdsLvl = 0;
    public int TVAdsLvl = 0;

    public BaseScript Basescript{get; private set;}
    public CustomerSpawner Spawner{get; private set;}

    [Export]
    public float CostIncreaseMultiplier = 2f;

    [Export]
    public float NewspaperCPMUpgradeSteps;
    [Export]
    public float NewspaperCostValue;
    [Export]
    public string NewspaperCostMagnitude;
    public Tuxdollar NewspaperCost;

    [Export]
    public float OnlineCPMUpgradeSteps;
    [Export]
    public float OnlineCostValue;
    [Export]
    public string OnlineCostMagnitude;
    public Tuxdollar OnlineCost;

    [Export]
    public float BillboardCPMUpgradeSteps;
    [Export]
    public float BillboardCostValue;
    [Export]
    public string BillboardCostMagnitude;
    public Tuxdollar BillboardCost;

    [Export]
    public float TVCPMUpgradeSteps;
    [Export]
    public float TVCostValue;
    [Export]
    public string TVCostMagnitude;
    public Tuxdollar TVCost;

    public Button NewspaperButton;
    public Label NewspaperCostLabel;

    public Button OnlineButton;
    public Label OnlineCostLabel;

    public Button BillboardButton;
    public Label BillboardCostLabel;

    public Button TVButton;
    public Label TVCostLabel;

    public float AdvertisementScore{get; private set;}

    

    public override void _Ready()
    {
        NewspaperCost = new Tuxdollar(NewspaperCostValue, NewspaperCostMagnitude);
        OnlineCost = new Tuxdollar(OnlineCostValue, OnlineCostMagnitude);
        BillboardCost = new Tuxdollar(BillboardCostValue, BillboardCostMagnitude);
        TVCost = new Tuxdollar(TVCostValue, TVCostMagnitude);

        Basescript = (BaseScript)GetParent();
        Spawner = (CustomerSpawner)Basescript.GetNode("Spawner");

        NewspaperButton = (Button)GetNode("NewspaperButton");
        NewspaperCostLabel = (Label)GetNode("NewspaperLabel");

        OnlineButton = (Button)GetNode("OnlineButton");
        OnlineCostLabel = (Label)GetNode("OnlineLabel");

        BillboardButton = (Button)GetNode("BillboardButton");
        BillboardCostLabel = (Label)GetNode("BillboardLabel");

        TVButton = (Button)GetNode("TVButton");
        TVCostLabel = (Label)GetNode("TVLabel");    
    }

    public void CheckButtonMode()
    {
        NewspaperButton.Disabled = Basescript.Money < NewspaperCost;
        OnlineButton.Disabled = Basescript.Money < OnlineCost;
        BillboardButton.Disabled = Basescript.Money < BillboardCost;
        TVButton.Disabled = Basescript.Money < TVCost;

    }

    public void UpdateText()
    {
        NewspaperCostLabel.Text = $"{NewspaperCost}";
        OnlineCostLabel.Text = $"{OnlineCost}";
        BillboardCostLabel.Text = $"{BillboardCost}";
        TVCostLabel.Text = $"{TVCost}";

        NewspaperButton.Text = (NewspaperAdsLvl == 0)? "Unlock": "Upgrade";
        OnlineButton.Text = (OnlineAdsLvl == 0)? "Unlock": "Upgrade";
        BillboardButton.Text = (BillboardAdsLvl == 0)? "Unlock": "Upgrade";
        TVButton.Text = (TVAdsLvl == 0)? "Unlock": "Upgrade";
        
    }

    private void _on_NewspaperButton_pressed()
    {
        Tuxdollar tempCost = NewspaperCost;
        NewspaperCost *= CostIncreaseMultiplier;
        Basescript.TransferMoney(-tempCost);
        NewspaperAdsLvl++;
        AdvertisementScore += NewspaperCPMUpgradeSteps;
        if(NewspaperAdsLvl == 1) AdvertisementScore += NewspaperCPMUpgradeSteps*3;
        _upgraded_Adds();
    }

    private void _on_OnlineButton_pressed()
    {
        Tuxdollar tempCost = OnlineCost;
        OnlineCost *= CostIncreaseMultiplier;
        Basescript.TransferMoney(-tempCost);
        OnlineAdsLvl++;
        AdvertisementScore += OnlineCPMUpgradeSteps;
        if(OnlineAdsLvl == 1) AdvertisementScore += OnlineCPMUpgradeSteps*3;
        _upgraded_Adds();
    }

    private void _on_BillboardButton_pressed()
    {
        Tuxdollar tempCost = BillboardCost;
        BillboardCost *= CostIncreaseMultiplier;
        Basescript.TransferMoney(-tempCost);
        NewspaperAdsLvl++;
        AdvertisementScore += BillboardCPMUpgradeSteps;
        if(NewspaperAdsLvl == 1) AdvertisementScore += BillboardCPMUpgradeSteps*3;
        _upgraded_Adds();
    }

    private void _on_TVButton_pressed()
    {
        Tuxdollar tempCost = TVCost;
        TVCost *= CostIncreaseMultiplier;
        Basescript.TransferMoney(-tempCost);
        NewspaperAdsLvl++;
        AdvertisementScore += TVCPMUpgradeSteps;
        if(NewspaperAdsLvl == 1) AdvertisementScore += TVCPMUpgradeSteps*3;
        _upgraded_Adds();
    }

    private void _on_AdvertisingManager_about_to_show()
    {
        UpdateText();
        CheckButtonMode();
    }

    private void _upgraded_Adds()
    {
        Spawner.ChangeWaitTime();
        UpdateText();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
