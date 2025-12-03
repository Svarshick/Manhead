using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicSpace.Cells
{
    public sealed class CellSide : MonoBehaviour
    {
        public Cell Cell { get; set; }
        public Direction Side { get; set; }
        public Dictionary<Type, CellSideComponent> Components { get; set; }
        public new T GetComponent<T>() where T : CellSideComponent
        {
            if (Components.TryGetValue(typeof(T), out var component))
                return (T) component;
            return null;
        }

        //TODO dirty hack
        void Awake() => gameObject.SetActive(Cell != null);

        public Direction GetLookDirection() => GetLookVector().ToDirection();
        public Vector2Int GetLookVector()
        {
            var cellLookVector = Cell.LookDirection.ToVector2Int();
            var x = cellLookVector.x;
            var y = cellLookVector.y;
            var transformMatrix = (y, -x, x, y);
            return MyMath.Multiply(Side.ToVector2Int(), transformMatrix);
        }

    }

    [RequireComponent(typeof(CellSide))]
    public class CellSideComponent : MonoBehaviour
    {
        public CellSide CellSide { get; set; }

        protected void Awake()
        {
            CellSide = GetComponent<CellSide>();
        }
    }
}