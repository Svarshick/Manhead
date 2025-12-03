using System;
using LogicSpace.Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace LogicSpace.Cells
{
    public class Moving : CellComponent
    {
        [Min(1)]
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public MovementType MovementType { get; set; }
        [field: SerializeField] public int Priority { get; set; }
        public MovementController MovementController => MovementTypeFabric.Create(MovementType, Cell.Field.Map, Cell.Field.GridPosition, Cell.LookDirection);
    }
}