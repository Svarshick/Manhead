using System;
using LogicSpace.Movement;
using UnityEngine;

namespace LogicSpace.Cells
{
    public class Moving : CellComponent
    {
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public MovementType MovementType { get; set; }
        public MovementController MovementController => MovementTypeFabric.Create(MovementType, Cell.Field.Map, Cell.Field.GridPosition, Cell.LookDirection);
    }
}