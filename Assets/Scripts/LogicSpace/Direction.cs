using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LogicSpace
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Ambiguous
    }
    
    public static class DirectionExtension
    {
        public static Vector2Int ToVector2Int(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector2Int.up,
                Direction.Down => Vector2Int.down,
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }
        
        public static Direction ToDirection(this Vector2 vector)
        {
            if (vector.y == 0)
            {
                if (vector.x > 0)
                    return Direction.Right;
                if (vector.x < 0)
                    return Direction.Left;
            }
            if (vector.x == 0)
            {
                if (vector.y > 0)
                    return Direction.Up;
                if (vector.y < 0)
                    return Direction.Down;
            }
            return Direction.Ambiguous;
        }
        
        public static Direction ToDirection(this Vector2Int vector)
        {
            if (vector.y == 0)
            {
                if (vector.x > 0)
                    return Direction.Right;
                if (vector.x < 0)
                    return Direction.Left;
            }
            if (vector.x == 0)
            {
                if (vector.y > 0)
                    return Direction.Up;
                if (vector.y < 0)
                    return Direction.Down;
            }
            return Direction.Ambiguous;
        }
        
        public static char ToArrow(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => '↑',
                Direction.Down => '↓',
                Direction.Left => '←',
                Direction.Right => '→',
                _ => '?'
            };
        }

        public static Direction GetOpposite(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.Ambiguous
            };
        }
    }
    
    public static class DirectionUtils
    {
        public static Direction GetRandom()
        {
            int index = Random.Range(0, 3);
            return (Direction)index;
        }
    }
}