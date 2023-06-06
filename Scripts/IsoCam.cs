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

    Vector3 minBounds = new Vector3(-10, -10, -10); // Minimum bounds for X, Y, and Z coordinates
    Vector3 maxBounds = new Vector3(10, 10, 10); // Maximum bounds for X, Y, and Z coordinates

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _parent = (BaseScript)GetParent();
        _camera = (Camera)GetNode("Camera");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch st)
        {
            if (@event.IsPressed())
            {
                events[st.Index] = new InputEventScreenDrag();
                events[st.Index].Position = st.Position;
                _parent.MaxInputDelay.Start();
                _parent.InputPosition = GetViewport().GetMousePosition();
            }
            else
            {
                events.Remove(st.Index);
            }

            if (events.Count == 2)
            {
                lastDragDistance = events[0].Position.DistanceTo(events[1].Position);
            }
        }

        if(events.Count == 0)
            _parent.IState = InputState.Default;

        if (@event is InputEventScreenDrag motionEvent)
        {
            events[motionEvent.Index] = motionEvent;

            if (events.Count == 2)
            {
                float dragDistance = events[0].Position.DistanceTo(events[1].Position);
                float newSize = (dragDistance < lastDragDistance) ? 1 + ZoomSpeed : 1 - ZoomSpeed;

                newSize = _camera.Size * newSize;

                if (newSize > ZoomMaximum) newSize = ZoomMaximum;
                if (newSize < ZoomMinimum) newSize = ZoomMinimum;

                _camera.Size = newSize;

                lastDragDistance = dragDistance;
            }

            if(_parent.InputPosition.DistanceTo(motionEvent.Position) > 30)
                _parent.IState =InputState.Dragging;

            if (_parent.IState != InputState.UIopened && events.Count == 1 && _parent.IState == InputState.Dragging)
            {
                Vector3 translation = new Vector3(motionEvent.Relative.x * -0.0015f * _camera.Size, 0, motionEvent.Relative.y * -0.0015f * _camera.Size);
                TranslateWithBounds(translation);
            }
        }

        if (@event is InputEventMouseButton mb && @event.IsPressed())
        {
            if (mb.ButtonIndex == (int)ButtonList.WheelUp && _camera.Size > ZoomMinimum)
                _camera.Size -= ZoomSpeed * 60;

            if (mb.ButtonIndex == (int)ButtonList.WheelDown && _camera.Size < ZoomMaximum)
                _camera.Size += ZoomSpeed * 60;
        }
    }

    private void TranslateWithBounds(Vector3 translation)
    {
        Vector3 newPosition = this.Translation + translation;
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
        newPosition.z = Mathf.Clamp(newPosition.z, minBounds.z, maxBounds.z);
        this.Translation = newPosition;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
