namespace LogicSpace.Movement
{
    public interface IMovementType
    {
        public Direction? GetNextStep();
    }
}