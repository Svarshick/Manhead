using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Manhead.Core.Logic.Editor;

public class EditorInput : IUpdatable
{
    public event Action<Vector2>? Draw;          // (ScreenPosition)
    public event Action<Vector2>? Erase;       // (ScreenPosition)
    
    public event Action<Vector2>? StartDrag;    // (StartScreenPosition)
    public event Action<Vector2, Vector2>? UpdateDrag; // (CurrentScreenPosition, DragDelta)
    public event Action<Vector2>? EndDrag;      // (EndScreenPosition)
    
    public event Action<float>? Zoom;           // (ScrollWheelDelta) - positive is scroll up/in, negative is scroll down/out
    private const float ZoomSensitivity = 0.003f;

    private readonly ButtonTracker _dragTracker = new();

    public EditorInput()
    {
        _dragTracker.DragStarted += pos => StartDrag?.Invoke(pos);
        _dragTracker.DragUpdated += (pos, delta) => UpdateDrag?.Invoke(pos, delta);
        _dragTracker.DragEnded += pos => EndDrag?.Invoke(pos);
    }
    
    public void Update()
    {
        var keyboardState = KeyboardExtended.GetState();
        var mouseState = MouseExtended.GetState();
        var currentMousePos = mouseState.Position.ToVector2();

        if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl))
        {
            _dragTracker.Process(mouseState.IsButtonDown(MouseButton.Left), currentMousePos);
        }
        else
        {
            _dragTracker.Process(false, currentMousePos);
            if (mouseState.IsButtonDown(MouseButton.Left))
            {
                Draw?.Invoke(currentMousePos);
            }
        }

        if (mouseState.IsButtonDown(MouseButton.Right))
        {
            Erase?.Invoke(currentMousePos);
        }

        if (mouseState.DeltaScrollWheelValue != 0)
        {
            var delta = ZoomSensitivity * mouseState.DeltaScrollWheelValue;
            Zoom?.Invoke(delta);
        }

        KeyboardExtended.Update();
        MouseExtended.Update();
    }

    public void LateUpdate()
    {
    }


    private class ButtonTracker
    {
        private enum State
        {
            Idle,
            Checking,
            Dragging
        }

        private State _state = State.Idle;
        private Vector2 _startPos;
        private Vector2 _lastPos;
        private readonly float _threshold;

        public event Action<Vector2>? Clicked;
        public event Action<Vector2>? DragStarted;
        public event Action<Vector2, Vector2>? DragUpdated;
        public event Action<Vector2>? DragEnded;

        public ButtonTracker(float threshold = 5f)
        {
            _threshold = threshold;
        }

        public void Process(bool isDown, Vector2 currentPos)
        {
            switch (_state)
            {
                case State.Idle:
                    if (isDown)
                    {
                        _startPos = currentPos;
                        _lastPos = currentPos;
                        _state = State.Checking;
                    }
                    break;

                case State.Checking:
                    if (!isDown)
                    {
                        Clicked?.Invoke(currentPos);
                        _state = State.Idle;
                    }
                    else if (Vector2.Distance(_startPos, currentPos) > _threshold)
                    {
                        _state = State.Dragging;
                        DragStarted?.Invoke(_startPos);
                        DragUpdated?.Invoke(currentPos, currentPos - _startPos);
                        _lastPos = currentPos;
                    }
                    break;

                case State.Dragging:
                    if (!isDown)
                    {
                        DragEnded?.Invoke(currentPos);
                        _state = State.Idle;
                    }
                    else
                    {
                        var delta = currentPos - _lastPos;
                        if (delta != Vector2.Zero)
                        {
                            DragUpdated?.Invoke(currentPos, delta);
                            _lastPos = currentPos;
                        }
                    }
                    break;
            }
        }
    }
}