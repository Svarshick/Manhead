using System.Collections.Generic;
using UnityEngine;

namespace LogicSpace.GameField
{
    public class Field
    {
        private readonly Grid _grid;
        private Dictionary<Vector2Int, Cell> _cells;

        public Field(Grid grid)
        {
            _grid = grid;
        }

        //NOTE: there could be problems because it is structure and property
        public BoundsInt Bounds { get; private set; }
        public IEnumerable<Cell> Cells => _cells.Values;

        public int Width => Bounds.max.x - Bounds.min.x + 1;
        public int Height => Bounds.max.y - Bounds.min.y + 1;
        public Vector3 WorldMin => new(Bounds.min.x * _grid.cellSize.x, Bounds.min.y * _grid.cellSize.y);
        public Vector3 WorldMax => new((Bounds.max.x + 1) * _grid.cellSize.x, (Bounds.max.y + 1) * _grid.cellSize.y);

        //TODO: temp and dirty
        public void SetCells(Dictionary<Vector2Int, Cell> cells, BoundsInt bounds)
        {
            _cells = cells;
            Bounds = bounds;
        }

        public Cell GetCell(Vector2Int position)
        {
            _cells.TryGetValue(position, out var cell);
            return cell;
        }

        public bool HasCellAt(Vector2Int position)
        {
            return _cells.ContainsKey(position);
        }

        public Vector3 GetCellCenterWorld(Vector3Int position)
        {
            return _grid.GetCellCenterWorld(position);
        }
    }
}