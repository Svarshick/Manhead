using System;
using System.Collections.Generic;
using LogicSpace.Fields;
using UnityEngine;

namespace LogicSpace.Cells
{
    public sealed class Cell : MonoBehaviour
    {
        [field: SerializeField] public CellSide LeftSide { get; set; }
        [field: SerializeField] public CellSide RightSide { get; set; }
        [field: SerializeField] public CellSide FrontSide { get; set; }
        [field: SerializeField] public CellSide BackSide { get; set; }
        
        public Field Field { get; set; }
        public Dictionary<Type, CellComponent> Components { get; set; }

        public new T GetComponent<T>() where T : CellComponent
        {
            if (Components.TryGetValue(typeof(T), out var component))
                return (T) component;
            return null;
        }

        private Direction _lookDirection = Direction.Up; 
        public Direction LookDirection
        {
            get => _lookDirection;
            set
            {
                _lookDirection = value;
                switch (_lookDirection)
                {
                    case Direction.Left: transform.localRotation = Quaternion.Euler(0, 0, 90); break;
                    case Direction.Right: transform.localRotation = Quaternion.Euler(0, 0, 270); break;
                    case Direction.Up: transform.localRotation = Quaternion.Euler(0, 0, 0); break;
                    case Direction.Down: transform.localRotation = Quaternion.Euler(0, 0, 180); break;
                }
            }
        }

        //TODO dirty hack
        void Awake() => gameObject.SetActive(Field != null);
        
        public void ChangeField(Field field)
        {
            Field?.Cells.Remove(this);
            Field = field ?? throw new ArgumentNullException(nameof(field));
            field.Cells.Add(this);
        }

        public CellSide GetSide(Direction direction) => 
            direction switch
            {
                Direction.Left => LeftSide,
                Direction.Right => RightSide,
                Direction.Up => FrontSide,
                Direction.Down => BackSide,
                _ => null
            };

        public CellSide GetSideRelative(Direction fromDirection)
        {
            var sideDirection = GetSideRelative(_lookDirection, fromDirection);
            return GetSide(sideDirection);
        }

        //TODO should it be math or just switch?
        //too much type casting
        public static Direction GetSideRelative(Direction lookDirection, Direction fromDirection)
        {
            var fromVector = fromDirection.ToVector2Int();
            var x = fromVector.x;
            var y = fromVector.y;
            var transformMatrix = (y, -x, -x, -y);
            return MyMath.Multiply(lookDirection.ToVector2Int(), transformMatrix).ToDirection();
        }
    }

    [RequireComponent(typeof(Cell))]
    public class CellComponent : MonoBehaviour
    {
        public Cell Cell { get; private set; }

        protected void Awake()
        {
            Cell = GetComponent<Cell>();
        }
    }
}