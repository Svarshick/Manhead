using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Gameplay.Movement;

public class AllTheWayMovement : MovementController
{
    public AllTheWayMovement(Point startPosition, Direction lookDirection) : base(startPosition, lookDirection)
    {
    }

    public override Step NextStep() => new Step(LookDirection, LookDirection);
}