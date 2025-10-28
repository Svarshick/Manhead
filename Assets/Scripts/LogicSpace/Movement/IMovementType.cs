using System;

namespace LogicSpace
{
    public interface IMovementType
    {
        public Direction? GetNextStep();
    }
}