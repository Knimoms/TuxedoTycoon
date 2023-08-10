using Godot;
using System;

public delegate void Timeout();
public class TitleScreen : Control
{
    public Label TitleLabel{get; private set;}
    public Label InstructionLabel{get; private set;}
    public Timer Timer{get; private set;}
    public BaseScript parent{get; private set;}
    public Timeout On_Timer_timeout;

    public override void _Ready()
    {
        On_Timer_timeout = firstTimeout;
        parent = (BaseScript)GetParent();
        TitleLabel = (Label)GetNode("TitleLabel");  
        InstructionLabel = (Label)GetNode("InstructionLabel");   
        Timer = (Timer)GetNode("Timer");
    }

    public void FadeOut(Control control, float duration)
    {
        SceneTreeTween tween = CreateTween();
        tween.TweenProperty(control, "modulate", new Color(control.Modulate, 0), duration);
    }

    public void FadeOut(Control control, float duration, Tween.TransitionType transitionType)
    {
        SceneTreeTween tween = CreateTween();
        tween.SetTrans(transitionType);
        tween.TweenProperty(control, "modulate", new Color(control.Modulate, 0), duration);
    }

    public void FadeOut(Control control, float duration, Godot.Object executingObject, string method)
    {
        SceneTreeTween tween = CreateTween();
        tween.TweenProperty(control, "modulate", new Color(control.Modulate, 0), duration);
        tween.TweenCallback(executingObject, method); 
    }

     public void FadeOut(Control control, float duration,Tween.TransitionType transitionType, Godot.Object executingObject, string method)
    {
        SceneTreeTween tween = CreateTween();
        tween.SetTrans(transitionType);
        tween.TweenProperty(control, "modulate", new Color(control.Modulate, 0), duration);     
        tween.TweenCallback(executingObject, method); 
    }

    public void FadeIn(Control control, float duration)
    {
        SceneTreeTween tween = CreateTween();
        tween.TweenProperty(control, "modulate", new Color(control.Modulate, 1), duration);
    }

    public void FadeIn(Control control, float duration, Tween.TransitionType transitionType)
    {
        SceneTreeTween tween = CreateTween();
        tween.SetTrans(transitionType);
        tween.TweenProperty(control, "modulate", new Color(control.Modulate, 1), duration);
    }

    public void FadeToggle(Control control, float duration, Tween.TransitionType transitionType)
    {
        if(control.Modulate.a == 1)
            FadeOut(control, duration, transitionType);
        else FadeIn(control, duration, transitionType);
    }

    private void _on_Timer_timeout()
    {
        On_Timer_timeout();
    }

    private void firstTimeout()
    {
        On_Timer_timeout = timeout;
        FadeIn(TitleLabel, 2);
    }

    private void timeout()
    {
        FadeToggle(InstructionLabel, 1, Tween.TransitionType.Circ);
    }

    public void LeaveTitleScreen()
    {
        Visible = false;
        parent.TitleScreenLeft();
    }

    public override void _Input(InputEvent @event)
    {
        if(parent.IState == InputState.StartScreen && @event is InputEventScreenTouch)
        {
            Timer.Stop();
            FadeOut(TitleLabel, 1, this, "LeaveTitleScreen");
            FadeOut(InstructionLabel, 1);

			Vector3 zoomTarget;
			if(parent.Restaurants.Count > 0)
				zoomTarget = parent.Restaurants[parent.Restaurants.Count - 1].Transform.origin;
			else
				zoomTarget = new Vector3(0.7f, -0.335f, -9.2f);
			
            parent.IsoCam.ZoomTo(zoomTarget, 6f, 1f);
            return;
        }
    }
}
