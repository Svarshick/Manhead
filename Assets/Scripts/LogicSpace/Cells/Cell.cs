using System;
using System.Collections.Generic;
using LogicSpace.Fields;
using UnityEngine;
using Math = DefaultNamespace.Math;

namespace LogicSpace.Cells
{
    public class Cell : MonoBehaviour
    {
        [field: SerializeField] public CellSide LeftSide { get; private set; }
        [field: SerializeField] public CellSide RightSide { get; private set; }
        [field: SerializeField] public CellSide FrontSide { get; private set; }
        [field: SerializeField] public CellSide BackSide { get; private set; }

        private Direction _lookDirection = Direction.Up;
        public Field Field { get; private set; }

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
        
        public Dictionary<Type, CellComponent> Components { get; private set; }

        void Awake()
        {
            Components = new (gameObject.GetComponentCount());
            foreach (var component in GetComponents<CellComponent>())
            {
                var componentType = component.GetType();
                if (Components.ContainsKey(componentType))
                    Debug.LogAssertion($"{this} contains more than one component of type {componentType}");
                Components[component.GetType()] = component;
            }
        }
        
        public void ChangeField(Field field)
        {
            Field?.Cells.Remove(this);
            Field = field ?? throw new ArgumentNullException(nameof(field));
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

    [RequireComponent(typeof(Cell))]
    public class CellComponent : MonoBehaviour
    {
        public Cell Cell { get; private set; }

        void Awake()
        {
            Cell = GetComponent<Cell>();
        }
    }
}