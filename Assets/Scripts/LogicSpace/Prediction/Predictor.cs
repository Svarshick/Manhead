using System;
using System.Collections.Generic;
using LogicSpace.Cells;
using LogicSpace.Movement;

namespace LogicSpace.Prediction
{
    public class Predictor
    {
        public List<IRequest> Predict(Cell cell, Step step)
        {
            var future = new List<IRequest>();
            var toField = cell.Field.GetNeighbour(step.stepDirection);
            if (toField is null)
                throw new ArgumentOutOfRangeException(
                    $"field {cell.Field.GridPosition + step.stepDirection.ToVector2Int()} does not exist");
            var cellSideDirection = Cell.GetSideRelative(step.lookDirection, step.stepDirection.GetOpposite());
            var cellSide = cell.GetSide(cellSideDirection);

            foreach (var toCell in toField.Cells)
            {
                var toCellSide = toCell.GetSideRelative(step.stepDirection);
                if (toCellSide.GetComponent<Stop>() != null)
                {
                    future.Add(new StopRequest{ target = cell });
                    return future;
                }
            }

            future.Add(new RotateRequest { target = cell, lookDirection = step.lookDirection });
            future.Add(new MoveRequest { target = cell, direction = step.stepDirection });
            foreach (var toCell in toField.Cells)
            {
                var toCellSide = toCell.GetSideRelative(step.stepDirection);
                if (toCellSide.GetComponent<CrossRoad>() != null)
                {
                    var crossRoad = toCellSide.GetComponent<CrossRoad>();
                    future.Add(new RotateRequest { target = cell, lookDirection = crossRoad.GlobalRotationDirection});
                }
            }

            return future;
        }
    }
}