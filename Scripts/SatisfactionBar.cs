using Godot;
using System;

public class SatisfactionBar : AnimatedSprite
{
    private BaseScript _base_script;
    private int[] _frame_numbers = new int[4];
    private CustomerMood _customer_mood = CustomerMood.NotEvaluated;
    public override void _Ready()
    {
        _frame_numbers[(int)CustomerMood.Happy] = 0;
        _frame_numbers[(int)CustomerMood.Neutral] = 16;
        _frame_numbers[(int)CustomerMood.Angry] = 29;
        _frame_numbers[(int)CustomerMood.NotEvaluated] = 38;
        _base_script = (BaseScript)GetParent().GetParent();
        _base_script.SatisfactionChanged += CheckSatisfaction;
    }

    private void CheckSatisfaction()
    {
        CustomerMood newMood = (CustomerMood)Math.Min((int)(_base_script.SatisfactionRating/33+1), 3);
        if(newMood == _customer_mood)
            return;

        _customer_mood = newMood;
        Frame = _frame_numbers[(int)_customer_mood];
        Playing = true;  
    }

    private void _on_SatisfactionBar_frame_changed()
    {
        if(Frame >= _frame_numbers[(int)_customer_mood-1]-1)
            Playing = false;
    }
}

public enum CustomerMood
{
    NotEvaluated,
    Angry,
    Neutral,
    Happy,
}
