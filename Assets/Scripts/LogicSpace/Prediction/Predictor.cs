using System.Collections.Generic;
using CustomMath;
using LogicSpace.GameEntity;
using LogicSpace.Movement;

namespace LogicSpace.Prediction
{
    public class Predictor
    {
        public List<IRequest> Predict(Entity entity, Step step)
        {
            var future = new List<IRequest>();
            var toCell = entity.Cell.GetNeighbour(step.stepDirection);
            if (toCell is null)
            {
                future.Add(new StopRequest { target = entity });
                return future;
            }

            foreach (var toEntity in toCell.Entities)
            {
                var toEntitySide = toEntity.GetSideVisibleFrom(step.stepDirection);
                if (toEntitySide.GetComponent<Stop>() != null)
                {
                    future.Add(new StopRequest { target = entity });
                    return future;
                }
            }

            future.Add(new RotateRequest { target = entity, lookDirection = step.lookDirection });
            future.Add(new MoveRequest { target = entity, direction = step.stepDirection });
            foreach (var toEntity in toCell.Entities)
            {
                var toCellSide = toEntity.GetSideVisibleFrom(step.stepDirection);
                var crossroadComponent = toCellSide.GetComponent<Crossroad>();
                if (crossroadComponent != null)
                {
                    var globalRotationDirection = SpaceUtility.GetGlobalDirection(crossroadComponent.rotationDirection,
                        toEntity.LookDirection);
                    future.Add(new RotateRequest { target = entity, lookDirection = globalRotationDirection });
                }
            }

            return future;
        }
    }
}