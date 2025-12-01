using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LogicSpace.Cells;
using LogicSpace.Fields;
using UnityEngine;

namespace LogicSpace.Movement
{
    public class MovementSystem
    {
        public static async UniTask Move(CancellationToken cancellationToken, Cell cell, Direction direction,
            float speed)
        {
            switch (direction)
            {
                case Direction.Up: await Move(cancellationToken, cell, Vector2Int.up, speed); break;
                case Direction.Down: await Move(cancellationToken, cell, Vector2Int.down, speed); break;
                case Direction.Left: await Move(cancellationToken, cell, Vector2Int.left, speed); break;
                case Direction.Right: await Move(cancellationToken, cell, Vector2Int.right, speed); break;
                default: throw new NotImplementedException();
            }
        }

        private static async UniTask Move(CancellationToken cancellationToken, Cell cell, Vector2Int direction,
            float speed)
        {
            var cellPosition = cell.Field.GridPosition;
            if (!cell.Field.Map.Fields.TryGetValue(cellPosition + direction, out var targetField))
                throw new ArgumentOutOfRangeException($"field {cellPosition + direction} does not exist");
            var targetPosition = targetField.WorldPosition;
            await MovementUtils.MoveTowards(cancellationToken, cell.gameObject, targetPosition, speed);
            cell.ChangeField(targetField);
        }

        public static bool CanMove(Cell cell, Direction direction)
        {
            var cellPosition = cell.Field.GridPosition;
            var directionVector = direction.ToVector2Int();
            return cell.Field.Map.Fields.ContainsKey(cellPosition + directionVector);
        }

        public static bool CanMove(Map map, Vector2Int fromPosition, Direction direction)
        {
            var directionVector = direction.ToVector2Int();
            return map.Fields.ContainsKey(fromPosition + directionVector);
        }
    }
}