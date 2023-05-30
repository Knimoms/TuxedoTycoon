using Godot;
using System;
using System.Collections.Generic;

public class IsoCam : Spatial
{
    private BaseScript _parent;

    float ZoomMinimum = 2f;
    float ZoomMaximum = 30f;
    float ZoomSpeed = 0.01f;
    private Camera _camera;
    Dictionary<int, InputEventScreenDrag> events = new Dictionary<int, InputEventScreenDrag>();
    float lastDragDistance;


    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _parent = (BaseScript)GetParent();
        _camera = (Camera)GetNode("Camera");
    }

    public override void _Input(InputEvent @event)
	{

        if(@event is InputEventScreenTouch st)
        {
            _parent.MaxInputDelay.Start();
            _parent.InputPosition = GetViewport().GetMousePosition();

            if(@event.IsPressed())
            {
                events[st.Index] = new InputEventScreenDrag();
                events[st.Index].Position = st.Position;
            } else
            {
                events.Remove(st.Index);
            }

            if(events.Count == 2)
            {
                lastDragDistance = events[0].Position.DistanceTo(events[1].Position);
            }
        }

        if(@event is InputEventScreenDrag motionEvent )
        {
            events[motionEvent.Index] = motionEvent;
            if( !_parent.UIopened && events.Count == 1)
            {
                this.Translate(new Vector3(motionEvent.Relative.x * -0.0015f*_camera.Size, 0, motionEvent.Relative.y * -0.0015f*_camera.Size));
            } 

            if(events.Count == 2)
            {
                float dragDistance = events[0].Position.DistanceTo(events[1].Position);
                float newSize = (dragDistance < lastDragDistance)? 1 + ZoomSpeed : 1 - ZoomSpeed;

                newSize = _camera.Size*newSize;

                if(newSize > ZoomMaximum) newSize = ZoomMaximum;
                if(newSize < ZoomMinimum) newSize = ZoomMinimum;

                _camera.Size = newSize;

                lastDragDistance = dragDistance;

            }
            
        }

        if(@event is InputEventMouseButton mb && @event.IsPressed())
        {
            if(mb.ButtonIndex == (int)ButtonList.WheelUp && _camera.Size > ZoomMinimum)
                _camera.Size -= ZoomSpeed*10;
            
            if(mb.ButtonIndex == (int)ButtonList.WheelDown && _camera.Size < ZoomMaximum)
                _camera.Size += ZoomSpeed*10;
            
            
        }       		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
