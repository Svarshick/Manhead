#nullable enable
using System.Collections.Generic;
using LogicSpace.Cells;
using UnityEngine;

namespace LogicSpace.Fields
{
    public class Field
    {
        public readonly Vector2Int GridPosition;
        public readonly Map Map;

        public HashSet<Cell> Cells = new();

        public Field(Map map, Vector2Int position)
        {
            Map = map;
            GridPosition = position;
        }

        public Vector3 WorldPosition => Map.GetCellCenterWorld((Vector3Int)GridPosition);
        public float Width => Map.Width; 
        public float Height => Map.Height;

        public Field? GetNeighbour(Direction direction)
        {
            return Map.Fields.GetValueOrDefault(GridPosition + direction.ToVector2Int());
        }
    }
}