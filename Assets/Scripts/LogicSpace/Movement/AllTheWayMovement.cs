using UnityEngine;

namespace LogicSpace
{
    public class AllTheWayMovement : IMovementType
    {
        private readonly FieldsGrid _fieldsGrid;
        private Vector2Int _currentPosition;
        private Direction _direction;

        public AllTheWayMovement(FieldsGrid fieldsGrid, Vector2Int position, Direction direction)
        {
            _fieldsGrid = fieldsGrid;
            _currentPosition = position;
            _direction = direction;
        }

        public Direction? GetNextStep()
        {
            if (!MovementSystem.CanMove(_fieldsGrid, _currentPosition, _direction))
                return null;
            _currentPosition += DirectionUtils.DirectionToVector2Int(_direction);
            return _direction;
        }
    }
}