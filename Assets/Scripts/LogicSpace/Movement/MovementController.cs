using System;
using LogicSpace.Fields;
using UnityEngine;

namespace LogicSpace.Movement
{
    public abstract class MovementController
    {
        protected readonly Map map;
        public Vector2Int CurrentPosition { get; set; }
        public Direction LookDirection { get; set; }

        protected MovementController(Map map, Vector2Int currentPosition,  Direction lookDirection)
        {
            this.map = map;
            CurrentPosition = currentPosition;
            LookDirection = lookDirection;
        }

        public abstract Step GetNextStep();
    }

    
    public struct Step
    {
        public Direction stepDirection;
        public Direction lookDirection;
    }
}