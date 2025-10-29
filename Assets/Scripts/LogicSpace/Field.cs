#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace LogicSpace
{
    public class Field
    {
        public readonly FieldsGrid Grid;
        public readonly Vector2Int GridPosition;
        public Vector3 WorldPosition => Grid.GetCellCenterWorld((Vector3Int) GridPosition);

        public HashSet<Cell.Cell> Cells = new();
        
        public Field(FieldsGrid grid, Vector2Int position)
        {
            Grid = grid;
            GridPosition = position;
        }

        public Field? GetNeighbour(Direction direction)
        {
            return Grid.Fields.GetValueOrDefault(GridPosition + direction.ToVector2Int());
        }
    }
}