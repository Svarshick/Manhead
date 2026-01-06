using System;
using CustomMath;
using UnityEngine;

namespace LogicSpace.Movement
{
    public enum MovementType
    {
        AllTheWayMovement
    }

    public static class MovementTypeFabric
    {
        public static MovementController Create(MovementType movementType, Vector2Int startPosition,
            Direction lookDirection)
        {
            return movementType switch
            {
                MovementType.AllTheWayMovement => new AllTheWayMovement(startPosition, lookDirection),
                _ => throw new NotImplementedException()
            };
        }
    }
}