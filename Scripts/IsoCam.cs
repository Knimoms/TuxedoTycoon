using Godot;
using System;
using System.Collections.Generic;

public delegate float Function(float x);
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

    [Export]
    public float ZoomTime = 1f;

    private float _zoom_time;
    private Vector3 _zoom_start;
    private float _size_start;
    private Vector3 _zoom_delta;
    private float _size_delta;

    public Function ZoomEffect;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ZoomEffect = Squared;
        _zoom_time = -1f;
        _parent = (BaseScript)GetParent();
        _camera = (Camera)GetNode("Camera");
    }

    public override void _Process(float delta)
    {
        if(_zoom_time > ZoomTime)
        {
            _zoom_time = -1f;
            if(_parent.IState == InputState.StartScreen)
            {
                _parent.IState = InputState.Default;
                if(_parent.OfflineReward > Tuxdollar.ZeroTux)
                    _parent._open_offlineReward_panel();
            }
        }

        if(_zoom_time >= 0f)
        {
            Transform modifiedTransform = Transform;
            float progress = ZoomEffect(_zoom_time/ZoomTime);
            modifiedTransform.origin = _zoom_start + _zoom_delta*progress;
            _camera.Size = _size_start + _size_delta*progress;
            Transform = modifiedTransform;
            _zoom_time += delta;
        }
    }

    public float Squared(float x) => x*x;

    public void ZoomTo(Vector3 targetPosition, float targetSize, float zoomTime)
    {
        ZoomTime = zoomTime;
        _zoom_time = 0f;
        _zoom_start = Transform.origin;
        _zoom_delta = targetPosition - _zoom_start;
        _zoom_delta.y = 0f;
        _size_start = _camera.Size;
        _size_delta = targetSize - _size_start;
    }

    public override void _Input(InputEvent @event)
    {

        if(_parent.IState == InputState.MiniGameOpened || _parent.IState == InputState.StartScreen)
            return;

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
            
            _parent.MaxInputDelay.Stop();

            if (events.Count == 2)
            {
                float dragDistance = events[0].Position.DistanceTo(events[1].Position);
                float newSize = (dragDistance < lastDragDistance) ? 1 + ZoomSpeed : 1 - ZoomSpeed;
                newSize = (float)Math.Pow(newSize, Math.Abs(lastDragDistance - dragDistance)*0.2);

                newSize = _camera.Size * newSize;

                if (newSize > ZoomMaximum) newSize = ZoomMaximum;
                if (newSize < ZoomMinimum) newSize = ZoomMinimum;

                _camera.Size = newSize;

                lastDragDistance = dragDistance;
            }

            if(_parent.InputPosition.DistanceTo(motionEvent.Position) > 30)
                _parent.IState = InputState.Dragging;

            if (events.Count == 1 && _parent.IState == InputState.Dragging)
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
