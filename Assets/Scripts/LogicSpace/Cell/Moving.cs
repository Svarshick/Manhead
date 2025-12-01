using LogicSpace.Movement;
using UnityEngine;

namespace LogicSpace.Cell
{
    [RequireComponent(typeof(Cell))]
    public class Moving : MonoBehaviour, ICellComponent
    {
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public MovementType MovementType { get; set; }
    }
}