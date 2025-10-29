using DefaultNamespace;
using UnityEngine;

namespace LogicSpace.Cell
{
    public class Cell : MonoBehaviour
    {
        public Field Field { get; private set; }
        [field: SerializeField] public CellSide LeftSide { get; private set; }
        [field: SerializeField] public CellSide RightSide { get; private set; }
        [field: SerializeField] public CellSide FrontSide { get; private set; }
        [field: SerializeField] public CellSide BackSide { get; private set; }
        public Direction LookDirection { get; set; }

        public void ChangeField(Field field)
        {
            Field?.Cells.Remove(this);
            Field = field ?? throw new System.ArgumentNullException(nameof(field));
            field.Cells.Add(this);
        }

        public CellSide GetSide(Direction direction)
        {
            return direction switch
            {
                Direction.Left => LeftSide,
                Direction.Right => RightSide,
                Direction.Up => FrontSide,
                Direction.Down => BackSide,
                _ => null
            };
        }

        //TODO should it be math or just switch?
        //too much type casting
        public CellSide GetSideRelative(Direction fromDirection)
        {
            var fromVector = fromDirection.ToVector2Int();
            var x = fromVector.x;
            var y = fromVector.y;
            var transformMatrix = (y, -x, -x, -y);
            var sideDirection = Math.Multiply(LookDirection.ToVector2Int(), transformMatrix).ToDirection();
            return GetSide(sideDirection);
        }
    }
}