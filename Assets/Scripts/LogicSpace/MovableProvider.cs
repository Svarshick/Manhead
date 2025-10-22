using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace LogicSpace
{
    public struct Movable : IComponent
    {
        public Rigidbody2D Rigidbody;
        public float Speed;
    }
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovableProvider : MonoProvider<Movable>
    {
        [SerializeField] private float speed;
        
        protected override void Initialize()
        {
            ref var data = ref GetData();
            data.Rigidbody = this.GetComponent<Rigidbody2D>();
            data.Speed = speed;
        }
    }
 
}