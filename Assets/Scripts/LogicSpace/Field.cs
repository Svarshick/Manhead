using System.Collections.Generic;
using UnityEngine;

namespace LogicSpace
{
    public class Field
    {
        public readonly FieldsGrid Grid;
        public readonly Vector2Int GridPosition;
        public Vector3 WorldPosition => Grid.GetCellCenterWorld((Vector3Int) GridPosition);

        public HashSet<Cell> Cells = new();
        
        public Field(FieldsGrid grid, Vector2Int position)
        {
            Grid = grid;
            GridPosition = position;
        }
    }
}