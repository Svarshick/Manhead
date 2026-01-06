using System;
using System.Threading;
using CustomMath;
using Cysharp.Threading.Tasks;
using LogicSpace.GameEntity;
using LogicSpace.GameField;
using UnityEngine;

namespace LogicSpace.Movement
{
    public class MovementSystem
    {
        public static async UniTask Move(CancellationToken cancellationToken, Entity entity, Direction direction,
            float speed)
        {
            switch (direction)
            {
                case Direction.Up: await Move(cancellationToken, entity, Vector2Int.up, speed); break;
                case Direction.Down: await Move(cancellationToken, entity, Vector2Int.down, speed); break;
                case Direction.Left: await Move(cancellationToken, entity, Vector2Int.left, speed); break;
                case Direction.Right: await Move(cancellationToken, entity, Vector2Int.right, speed); break;
                default: throw new NotImplementedException();
            }
        }

        private static async UniTask Move(CancellationToken cancellationToken, Entity entity, Vector2Int direction,
            float speed)
        {
            var cellPosition = entity.Cell.GridPosition;
            var targetCell = entity.Cell.Field.GetCell(cellPosition + direction);
            if (targetCell == null)
                throw new ArgumentOutOfRangeException($"cell {cellPosition + direction} does not exist");
            var targetPosition = targetCell.WorldPosition;
            await MovementUtils.MoveTowards(cancellationToken, entity.Appearance, targetPosition, speed);
            entity.ChangeCell(targetCell);
        }

        public static bool CanMove(Entity entity, Direction direction)
        {
            var cellPosition = entity.Cell.GridPosition;
            var directionVector = direction.ToVector2Int();
            return entity.Cell.Field.HasCellAt(cellPosition + directionVector);
        }

        public static bool CanMove(Field field, Vector2Int fromPosition, Direction direction)
        {
            var directionVector = direction.ToVector2Int();
            return field.HasCellAt(fromPosition + directionVector);
        }
    }
}