using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Manhead.Core.Logic.Gameplay;

public class GameInput : IUpdatable
{
    public event Action<Direction> Move; // (Direction)
   
    public void Update()
    {
        KeyboardExtended.Update();
        MouseExtended.Update();
        var keyboardState = KeyboardExtended.GetState();
        var mouseState = MouseExtended.GetState();
        UpdateMove(mouseState, keyboardState);
    }
    
    public void LateUpdate() { }

    private void UpdateMove(
        MouseStateExtended mouseState,
        KeyboardStateExtended keyboardState)
    {
        if (keyboardState.WasKeyPressed(Keys.W) ||
            keyboardState.WasKeyPressed(Keys.Up))
        {
            Move?.Invoke(Direction.Up);
            return;
        }

        if (keyboardState.WasKeyPressed(Keys.S) ||
            keyboardState.WasKeyPressed(Keys.Down))
        {
            Move?.Invoke(Direction.Down);
            return;
        }

        if (keyboardState.WasKeyPressed(Keys.D) ||
            keyboardState.WasKeyPressed(Keys.Right))
        {
            Move?.Invoke(Direction.Right);
            return;
        }

        if (keyboardState.WasKeyPressed(Keys.A) ||
            keyboardState.WasKeyPressed(Keys.Left))
        {
            Move?.Invoke(Direction.Left);
            return;
        }
    }
}