using LogicSpace;
using UnityEngine;

public static class MyMath
{
    public static Vector2Int Multiply(Vector2Int vector, (int x0, int y0, int x1, int y1) matrix)
    {
        return new Vector2Int(
            vector.x * matrix.x0 + vector.y * matrix.x1,
            vector.x * matrix.y0 + vector.y * matrix.y1);
    }
    
    public static Vector2 Multiply(Vector2 vector, (int x0, int y0, int x1, int y1) matrix)
    {
        return new Vector2(
            vector.x * matrix.x0 + vector.y * matrix.x1,
            vector.x * matrix.y0 + vector.y * matrix.y1);
    }
    
    public static Vector2 TurnVector(Vector2 vector, Direction rotationDirection)
    {
        var rotationVector = rotationDirection.ToVector2Int();
        var x = rotationVector.x;
        var y = rotationVector.y;
        var transformMatrix = (y, -x, x, y);
        return Multiply(vector, transformMatrix);
    }
    
    public static Vector2Int TurnVector(Vector2Int vector, Direction rotationDirection)
    {
        var rotationVector = rotationDirection.ToVector2Int();
        var x = rotationVector.x;
        var y = rotationVector.y;
        var transformMatrix = (y, -x, x, y);
        return Multiply(vector, transformMatrix);
    }
}