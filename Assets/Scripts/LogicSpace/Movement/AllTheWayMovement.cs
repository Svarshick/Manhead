using LogicSpace.Fields;
using UnityEngine;

namespace LogicSpace.Movement
{
    public class AllTheWayMovement : MovementController
    {
        public AllTheWayMovement(Map map, Vector2Int startPosition, Direction lookDirection): base (map, startPosition, lookDirection) { }
        
        public override Step GetNextStep()
        {
            if (!MovementSystem.CanMove(map, CurrentPosition, LookDirection))
                return new Step
                {
                    lookDirection = LookDirection, stepDirection = Direction.Ambiguous
                }; //TODO rename Direction.Ambiguous?
            CurrentPosition += LookDirection.ToVector2Int();
            return new Step { lookDirection = LookDirection, stepDirection = LookDirection };
        }
    }
}