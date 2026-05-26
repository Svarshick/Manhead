using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Gameplay.Movement;

public enum MovementType
{
    AllTheWayMovement
}

public static class MovementTypeFabric
{
    public static MovementController Create(MovementType movementType, Point startPosition,
        Direction lookDirection)
    {
        return movementType switch
        {
            MovementType.AllTheWayMovement => new AllTheWayMovement(startPosition, lookDirection),
            _ => throw new NotImplementedException()
        };
    }
}