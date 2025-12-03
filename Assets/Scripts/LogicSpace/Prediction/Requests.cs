using LogicSpace.Cells;

namespace LogicSpace.Prediction
{
    public interface IRequest
    {
    }

    public struct MoveRequest : IRequest
    {
        public Cell target;
        public Direction direction;
    }

    public struct RotateRequest : IRequest
    {
        public Cell target;
        public Direction lookDirection;
    }

    public struct StopRequest : IRequest
    {
        public Cell target;
    }
}