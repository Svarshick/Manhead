using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Gameplay.Movement;

public abstract class MovementController
{
    protected MovementController(Point currentPosition, Direction lookDirection)
    {
        CurrentPosition = currentPosition;
        LookDirection = lookDirection;
    }

    public Point CurrentPosition { get; set; }
    public Direction LookDirection { get; set; }

    public abstract Step NextStep();
}

public readonly struct Step
{
    public readonly Direction StepDirection;
    public readonly Direction LookDirection;

    public Step(Direction stepDirection, Direction lookDirection)
    {
        StepDirection = stepDirection;
        LookDirection = lookDirection;
    }
}