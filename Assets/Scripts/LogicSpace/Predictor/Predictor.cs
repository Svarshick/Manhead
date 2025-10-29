using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpace.Cell;

namespace LogicSpace.Predictor
{
    public interface IRequest {}

    public struct MoveRequest : IRequest
    {
        public Cell.Cell target;
        public Direction direction;
    }

    public struct StopRequest : IRequest
    {
        public Cell.Cell target;
    }
    
    public class Predictor
    {
        public IEnumerable<IRequest> Predict(Cell.Cell cell, Direction direction)
        {
            var toField = cell.Field.GetNeighbour(direction);
            if (toField is null)
                throw new ArgumentOutOfRangeException($"field {cell.Field.GridPosition + direction.ToVector2Int()} does not exist");
            
            if (toField.Cells.Count == 0)
            {
                yield return new MoveRequest() { target = cell, direction = direction };
                yield break;
            }
            var facedCell = toField.Cells.First();
            var stopComponent = facedCell.GetComponent<Stop>();
            if (stopComponent is not null)
            {
                yield return new StopRequest() {target = cell};
                yield break;
            }

            yield return new MoveRequest() { target = cell, direction = direction };
        }
    }
}