using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Manhead.Core.Logic.WorldSpace;

public class GridLayout
{
    public readonly Vector2 CellSize;

    public GridLayout(Vector2 cellSize)
    {
        CellSize = cellSize;
    }

    public Point WorldToGrid(Vector2 worldPosition)
    {
        var x = (int)Math.Floor(worldPosition.X / CellSize.X);
        var y = (int)Math.Floor(worldPosition.Y / CellSize.Y);
        return new Point(x, y);
    }

    public Vector2 GridToWorld(Point gridPosition)
    {
        var x = gridPosition.X * CellSize.X + CellSize.X / 2;
        var y = gridPosition.Y * CellSize.Y + CellSize.Y / 2;
        return new Vector2(x, y);
    }

    public RectangleF GridRectangle(Point gridPosition)
    {
        var x = gridPosition.X * CellSize.X;
        var y = gridPosition.Y * CellSize.Y;
        return new RectangleF(x, y, CellSize.X, CellSize.Y);
    }
}