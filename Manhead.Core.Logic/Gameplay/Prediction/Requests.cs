using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.WorldSpace;

namespace Manhead.Core.Logic.Gameplay.Prediction;

public interface IRequest
{
}

public struct MoveRequest : IRequest
{
    public Entity target;
    public Direction direction;
}

public struct RotateRequest : IRequest
{
    public Entity target;
    public Direction lookDirection;
}

public struct StopRequest : IRequest
{
    public Entity target;
}