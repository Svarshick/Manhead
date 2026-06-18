using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using Manhead.Core.Logic.Gameplay.Movement;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Gameplay.Prediction;

public class Predictor
{
    private Field<Entity> _entityField;
    public Predictor(Field<Entity> entityField)
    {
        _entityField = entityField;
    }
    
    public List<IRequest> Predict(Entity entity, Step step)
    {
        var future = new List<IRequest>();
        
        GetNeighbours(entity.Position, step.StepDirection, out var neighbours);
        if (neighbours is null)
        {
            future.Add(new StopRequest { target = entity });
            return future;
        }

        foreach (var toEntity in neighbours)
        {
            var toEntitySide = toEntity.GetSide(Space.SideVisibleFrom(step.LookDirection, toEntity.LookDirection));
            if (toEntitySide.GetComponent<Wall>() != null)
            {
                future.Add(new StopRequest { target = entity });
                return future;
            }
        }

        future.Add(new RotateRequest { target = entity, lookDirection = step.LookDirection });
        future.Add(new MoveRequest { target = entity, direction = step.StepDirection });
        /*foreach (var toEntity in neighbours)
        {
            var toEntitySide = toEntity.GetSide(Space.SideVisibleFrom(step.LookDirection, toEntity.LookDirection));
            var crossroadComponent = toEntitySide.GetComponent<Crossroad>();
            if (crossroadComponent != null)
            {
                var globalRotationDirection = Space.GetGlobalDirection(
                    crossroadComponent.RotationDirection,
                    toEntity.LookDirection);
                future.Add(new RotateRequest { target = entity, lookDirection = globalRotationDirection });
            }
        }*/

        return future;
    }

    private void GetNeighbours(Point position, Direction direction, out List<Entity>? neighbours)
    {
        neighbours = null;
        if (_entityField.InBounds(position + direction.ToPoint()))
        {
            neighbours = _entityField[position + direction.ToPoint()];
        }
    }
}