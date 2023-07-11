using Godot;
using System;
using System.Collections.Generic;

public class AdvertisingManager : PopupMenu
{
    public int NewspaperAdsLvl = 0;
    // public int OnlineAdsLvl = 0;
    // public int BillboardAdsLvl = 0;
    // public int TVAdsLvl = 0;

    public BaseScript Basescript{get; private set;}
    public CustomerSpawner Spawner{get; private set;}

    [Export]
    public int BadRatingMax = 34;
    [Export]
    public int GoodRatingMin = 67;
    [Export]
    public float CostIncreaseMultiplier = 2f;

    [Export]
    public float NewspaperCPMUpgradeSteps;
    [Export]
    public float NewspaperCostValue;
    [Export]
    public string NewspaperCostMagnitude;
    public Tuxdollar NewspaperCost;

    public Button NewspaperButton;
    public Label NewspaperCostLabel;
    public Label AdNameLabel;

    public float AdvertisementScore;
    private Timer _adTimer;
    private bool _adIsActive = true;
    private bool _isUpgraded = false;

    private float _ad_time = 0;
    private float? SavedAdTimeLeft;


    private List<string> _adNames = new List<string>()
    {
        "TV Ad",
        "Online Ad",
        "Billboard Ad",
        "Radio Ad",
        "Social Media Ad",
        "Newspaper Ad"
    };
    private int _currentAdIndex = 0;
    //private bool _updateAdName = false;
    

    public override void _Ready()
    {
        NewspaperCost = new Tuxdollar(NewspaperCostValue, NewspaperCostMagnitude);

        Basescript = (BaseScript)GetParent();
        Basescript.MoneyTransfered += CheckButtonMode;
        Spawner = (CustomerSpawner)Basescript.GetNode("Spawner");

        NewspaperButton = (Button)GetNode("NewspaperButton");
        NewspaperCostLabel = (Label)GetNode("NewspaperLabel");

        AdNameLabel = (Label)GetNode("AdNameLabel");

        _adTimer = (Timer)GetNode("AdTimer");   
        _ad_time = _adTimer.WaitTime;
        _adTimer.WaitTime = (SavedAdTimeLeft != null)? (float)SavedAdTimeLeft : _ad_time;

        AdNameLabel.Text = _adNames[_currentAdIndex];   
    }

    public void CheckButtonMode()
    {
        NewspaperButton.Disabled = !_adIsActive || Basescript.Money < NewspaperCost;
    }

    public void UpdateText()
    {
        NewspaperCostLabel.Text = $"{NewspaperCost}";
        NewspaperButton.Text = "Buy";
    }

    private void _on_NewspaperButton_pressed()
    {
        //Tuxdollar tempCost = NewspaperCost;
        Basescript.TransferMoney(-NewspaperCost);

        _adTimer.Start();
        _adIsActive = false;
        _UpdateButtonState();
         Basescript.CalculateCustomersPerMinute();
    }

    private void _UpdateButtonState()
    {
        if (_adIsActive)
        {
            NewspaperButton.Text = "Buy";
            //NewspaperButton.Text = (NewspaperAdsLvl == 0) ? "Unlock" : "Buy";
        }
        else
        {
            NewspaperButton.Text = "Is Advertising";
        }
        NewspaperButton.Disabled = !_adIsActive || Basescript.Money < NewspaperCost;
    }

    private void _on_AdvertisingManager_about_to_show()
    {
        UpdateText();
        CheckButtonMode();
        _UpdateButtonState();
    }

    private void _upgraded_Adds()
    {
        if (!_adIsActive) return;

        if (!_isUpgraded)
        {
            Spawner.ChangeWaitTime();
            AdvertisementScore += NewspaperCPMUpgradeSteps;
            if (NewspaperAdsLvl == 1) AdvertisementScore += NewspaperCPMUpgradeSteps * 3;
            UpdateText();
        }
    }

    public void _on_AdTimer_timeout()
    {
        if(_adTimer.WaitTime != _ad_time)
            _adTimer.WaitTime = _ad_time;

        NewspaperCost *= CostIncreaseMultiplier;
        NewspaperAdsLvl++;
        AdvertisementScore += NewspaperCPMUpgradeSteps;
        if(NewspaperAdsLvl == 1) AdvertisementScore += NewspaperCPMUpgradeSteps*3;
        _isUpgraded = true;
        _upgraded_Adds();

        _adIsActive = true;
        _UpdateButtonState();

        if (_isUpgraded)
        {
            Spawner.ChangeWaitTime();
            AdvertisementScore -= NewspaperCPMUpgradeSteps;
            if (NewspaperAdsLvl == 1) AdvertisementScore -= NewspaperCPMUpgradeSteps*3;
            _isUpgraded = false;
        }
        _adTimer.Stop();
        //_updateAdName = true;
        _currentAdIndex = (_currentAdIndex + 1) % _adNames.Count;
        AdNameLabel.Text = _adNames[_currentAdIndex];
        Basescript.CalculateCustomersPerMinute();
        UpdateText();
    }

    public Dictionary<string, object> Save()
	{
		return new Dictionary<string, object>()
		{
			{"Filename", Filename},
			{"Parent", GetParent().GetPath()},
            {"NewspaperCostValue", NewspaperCost.Value},
            {"NewspaperCostMagnitude", NewspaperCost.Magnitude},
        	{"BadRatingMax", BadRatingMax},
            {"GoodRatingMin", GoodRatingMin},
            {"CostIncreaseMultiplier", CostIncreaseMultiplier},
            {"NewspaperCPMUpgradeSteps", NewspaperCPMUpgradeSteps},
            {"AdvertisementScore", AdvertisementScore},
            {"_currentAdIndex", _currentAdIndex},
            {"SavedAdTimeLeft", _adTimer.TimeLeft}
        };
		
	}
}