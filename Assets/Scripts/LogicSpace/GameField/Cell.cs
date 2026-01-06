using System.Collections.Generic;
using CustomMath;
using LogicSpace.GameEntity;
using UnityEngine;

namespace LogicSpace.GameField
{
    public class Cell
    {
        public readonly List<Entity> Entities;
        public readonly Field Field;
        public readonly Vector2Int GridPosition;

        public Cell(Field field, Vector2Int gridPosition)
        {
            Field = field;
            GridPosition = gridPosition;
            Entities = new List<Entity>();
        }

        public Vector3 WorldPosition => Field.GetCellCenterWorld((Vector3Int)GridPosition);

        public Cell GetNeighbour(Direction direction)
        {
            return Field.GetCell(GridPosition + direction.ToVector2Int());
        }
    }
}