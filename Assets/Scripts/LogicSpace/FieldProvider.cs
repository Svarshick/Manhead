using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace LogicSpace
{
    public struct Field : IComponent
    {
        public Transform Transform;
    }
    
    public class FieldProvider : MonoProvider<Field>
    {
        protected override void Initialize()
        {
            ref var data = ref GetData();
            data.Transform = this.transform;
        }
    }
    
}