using CustomMath;
using UnityEngine;

namespace LogicSpace.Movement
{
    public class AllTheWayMovement : MovementController
    {
        public AllTheWayMovement(Vector2Int startPosition, Direction lookDirection) : base(startPosition, lookDirection)
        {
        }

        public override Step GetNextStep()
        {
            return new Step { lookDirection = LookDirection, stepDirection = LookDirection };
        }
    }
}