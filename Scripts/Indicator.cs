using Godot;
using System;

public class Indicator : Sprite3D
{
    double Time = 0.0;
    
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        Time += delta;

        Scale = new Vector3(1,1,0)*(float)(Math.Sin(Time*2)*0.25f+2);
    }

}
