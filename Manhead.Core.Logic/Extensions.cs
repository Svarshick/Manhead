using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;

namespace Manhead.Core.Logic;

public static class Extensions
{
    public static Color Color(this Random random, int alpha = 255)
    {
        return new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), alpha);
    }

    public static bool IsAnyButtonDown(this MouseStateExtended mouseState)
    {
        return mouseState.IsButtonDown(MouseButton.Left) ||
               mouseState.IsButtonDown(MouseButton.Middle) || 
               mouseState.IsButtonDown(MouseButton.Right) ||
               mouseState.IsButtonDown(MouseButton.XButton1) ||
               mouseState.IsButtonDown(MouseButton.XButton2);
    }

    public static bool IsAnyButtonDown(this KeyboardStateExtended keyboardState)
    {
        return keyboardState.GetPressedKeyCount > 0;
    }
}