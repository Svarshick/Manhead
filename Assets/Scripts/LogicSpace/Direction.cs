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

    public static class DirectionUtils
    {
        public static Direction GetRandom()
        {
            int index = Random.Range(0, 3);
            return (Direction)index;
        }
        
        public static Direction Vector2ToDirection(Vector2 vector)
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

        public static Vector2Int DirectionToVector2Int(Direction direction)
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
    }
}