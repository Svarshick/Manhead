using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
                default: throw new System.NotImplementedException();
            }
        }

        public static async UniTask Move(CancellationToken cancellationToken, Cell cell, Vector2 direction)
        {
            if (direction == Vector2.up) await Move(cancellationToken, cell, Vector2Int.up);
            else if (direction == Vector2.down) await Move(cancellationToken, cell, Vector2Int.down);
            else if (direction == Vector2.left) await Move(cancellationToken, cell, Vector2Int.left);
            else if (direction == Vector2.right) await Move(cancellationToken, cell, Vector2Int.right);
            else throw new System.NotImplementedException();
        }

        private static async UniTask Move(CancellationToken cancellationToken, Cell cell, Vector2Int direction)
        {
            var cellPosition = cell.Field.GridPosition;
            var targetField = cell.Field.Grid.Fields[cellPosition + direction];
            var targetPosition = targetField.WorldPosition;
            await MovementUtils.MoveTowards(cancellationToken, cell.gameObject, targetPosition, cell.Speed);
            cell.ChangeField(targetField);
        }
    }
}