using UnityEngine;

namespace LogicSpace.Movement
{
    public class AllTheWayMovement : IMovementType
    {
        private readonly Map _map;
        private Vector2Int _currentPosition;
        private Direction _direction;

        public AllTheWayMovement(Map map, Vector2Int position, Direction direction)
        {
            _map = map;
            _currentPosition = position;
            _direction = direction;
        }

        public Direction? GetNextStep()
        {
            if (!MovementSystem.CanMove(_map, _currentPosition, _direction))
                return null;
            _currentPosition += _direction.ToVector2Int();
            return _direction;
        }
    }
}