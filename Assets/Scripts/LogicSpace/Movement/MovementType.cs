using System;
using LogicSpace.Fields;
using UnityEngine;

namespace LogicSpace.Movement
{
    public enum MovementType
    {
        AllTheWayMovement
    }

    public static class MovementTypeFabric
    {
        public static MovementController Create(MovementType movementType, Map map, Vector2Int startPosition,
            Direction lookDirection)
        {
            return movementType switch
            {
                MovementType.AllTheWayMovement => new AllTheWayMovement(map, startPosition, lookDirection),
                _ => throw new NotImplementedException()
            };
        }
    }
}