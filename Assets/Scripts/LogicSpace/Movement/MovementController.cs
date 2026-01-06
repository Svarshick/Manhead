using CustomMath;
using UnityEngine;

namespace LogicSpace.Movement
{
    public abstract class MovementController
    {
        protected MovementController(Vector2Int currentPosition, Direction lookDirection)
        {
            CurrentPosition = currentPosition;
            LookDirection = lookDirection;
        }

        public Vector2Int CurrentPosition { get; set; }
        public Direction LookDirection { get; set; }

        public abstract Step GetNextStep();
    }


    public struct Step
    {
        public Direction stepDirection;
        public Direction lookDirection;
    }
}