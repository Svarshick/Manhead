using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Gameplay.Movement
{
    public class MovementService
    {
        private readonly Field<Entity> _field;
        private GridLayout _gridLayout;
        
        public MovementService(Field<Entity> field, GridLayout gridLayout)
        {
            _field = field;
            _gridLayout = gridLayout;
        }

        private async Task Move(Entity entity, Transform view, Direction direction, float speed)
        {
            var oldPosition = _gridLayout.WorldToGrid(view.Position);
            var newPosition = oldPosition + direction.ToPoint();
            if (!_field.InBounds(newPosition))
                throw new ArgumentOutOfRangeException($"cell in {newPosition} does not exist");
            
            var newWorldPosition = _gridLayout.GridToWorld(newPosition);
            await MovementUtils.MoveTowards(view, newWorldPosition, speed);
            
            var oldCell = _field[oldPosition];
            var newCell = _field[newPosition];
            oldCell.Remove(entity);
            newCell.Add(entity);
        }

        public bool CanMove(Point fromPosition, Direction toDirection)
        {
            var toPosition = fromPosition + toDirection.ToPoint();
            return _field.InBounds(toPosition);
        }
    }
}