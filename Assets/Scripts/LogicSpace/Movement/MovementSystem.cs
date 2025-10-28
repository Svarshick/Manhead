using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace LogicSpace
{
    public class MovementSystem
    {
        public static async UniTask Move(CancellationToken cancellationToken, Cell cell, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: await Move(cancellationToken, cell, Vector2Int.up); break;
                case Direction.Down: await Move(cancellationToken, cell, Vector2Int.down); break;
                case Direction.Left: await Move(cancellationToken, cell, Vector2Int.left); break;
                case Direction.Right: await Move(cancellationToken, cell, Vector2Int.right); break;
                default: throw new NotImplementedException();
            }
        }

        private static async UniTask Move(CancellationToken cancellationToken, Cell cell, Vector2Int direction)
        {
            var cellPosition = cell.Field.GridPosition;
            if (!cell.Field.Grid.Fields.TryGetValue(cellPosition + direction, out var targetField))
            {
                throw new ArgumentOutOfRangeException($"field {cellPosition + direction} does not exist");
            }
            var targetPosition = targetField.WorldPosition;
            await MovementUtils.MoveTowards(cancellationToken, cell.gameObject, targetPosition, cell.Speed);
            cell.ChangeField(targetField);
        }

        public static bool CanMove(Cell cell, Direction direction)
        {
            var cellPosition = cell.Field.GridPosition;
            var directionVector = DirectionUtils.DirectionToVector2Int(direction);
            return cell.Field.Grid.Fields.ContainsKey(cellPosition + directionVector);
        }

        public static bool CanMove(FieldsGrid fieldsGrid, Vector2Int fromPosition, Direction direction)
        {
            var directionVector = DirectionUtils.DirectionToVector2Int(direction);
            return fieldsGrid.Fields.ContainsKey(fromPosition + directionVector);
        }
    }
}