using UnityEngine;


namespace DefaultNamespace
{
    public static class Math
    {
        public static Vector2Int Multiply(Vector2Int vector, (int x0, int y0, int x1, int y1) matrix)
        {
            return new Vector2Int(
                vector.x * matrix.x0 + vector.y * matrix.x1,
                vector.x * matrix.y0 + vector.y * matrix.y1);
        }
    }
}