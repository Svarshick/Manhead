using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace LogicSpace
{    
    public struct Cell : IComponent
    {
        public Collider2D Collider;
        public Transform Transform;
        public Entity Field;
    }


    [RequireComponent(typeof(Collider2D))]
    public class CellProvider : MonoProvider<Cell>
    {
        protected override void Initialize()
        {
            ref var data = ref GetData();
            data.Collider = this.GetComponent<Collider2D>();
            data.Transform = this.transform;
        }
    }
}