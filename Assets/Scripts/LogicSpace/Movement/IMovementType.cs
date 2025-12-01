using System;
using UnityEngine;

namespace LogicSpace.Movement
{
    public interface IMovementType
    {
        public Direction? GetNextStep();
    }

    public enum MovementType
    {
        AllTheWayMovement
    }

    public static class MovementTypeFabric 
    {
        public static IMovementType Create(MovementType movementType, Map grid, Vector2Int startPosition, Direction direction)
        {
            return movementType switch
            {
                MovementType.AllTheWayMovement => new AllTheWayMovement(grid, startPosition, direction),
                _ => throw new NotImplementedException()
            };
        }
    }
}