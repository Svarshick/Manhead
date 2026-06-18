using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.WorldSpace;

public static class Space
{
    //Y reverse. -Y up; +Y down
    public static Point PointUp => new(0, -1);
    public static Point PointDown => new(0, 1);
    public static Point PointLeft => new(-1, 0);
    public static Point PointRight => new(1, 0);
    public static Vector2 Vector2Up => new(0, -1);
    public static Vector2 Vector2Down => new(0, 1);
    public static Vector2 Vector2Left => new(-1, 0);
    public static Vector2 Vector2Right => new(1, 0);
    
    public static Direction GetGlobalDirection(Direction direction, Direction lookDirection)
    {
        return (Direction)(((int)direction + (int)lookDirection) % 4);
    }

    public static Direction SideVisibleFrom(Direction fromDirection, Direction objectLookDirection)
    {
        //get fromDirection' for object system coordinates by -objectLookDirection
        //get opposite to fromDirection' (actual side) by fromDirection'+2
        return (Direction)(((int)fromDirection - (int)objectLookDirection + 2) % 4);
    }
}