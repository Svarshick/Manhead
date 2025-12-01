using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpace.Cells;
using LogicSpace.Movement;

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

    public class Predictor
    {
        public IEnumerable<IRequest> Predict(Cell cell, Step step)
        {
            var toField = cell.Field.GetNeighbour(step.stepDirection);
            if (toField is null)
                throw new ArgumentOutOfRangeException(
                    $"field {cell.Field.GridPosition + step.stepDirection.ToVector2Int()} does not exist");

            if (toField.Cells.Count == 0)
            {
                yield return new RotateRequest {target = cell, lookDirection = step.lookDirection};
                yield return new MoveRequest { target = cell, direction = step.stepDirection };
                yield break;
            }

            var facedCell = toField.Cells.First();
            var stopComponent = facedCell.GetComponent<Stop>();
            if (stopComponent is not null)
            {
                yield return new StopRequest { target = cell };
                yield break;
            }

            yield return new RotateRequest {target = cell, lookDirection = step.lookDirection};
            yield return new MoveRequest { target = cell, direction = step.stepDirection };
        }
    }
}