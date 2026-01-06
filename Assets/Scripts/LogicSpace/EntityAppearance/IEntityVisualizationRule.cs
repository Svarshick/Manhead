using System.Collections.Generic;
using LogicSpace.GameEntity;
using UnityEngine;

namespace LogicSpace.EntityAppearance
{
    public class EntityVisualizationContext
    {
        public HashSet<object> ConsumedComponents = new();
        public GameObject RootGO;

        public bool IsConsumed(object component)
        {
            return ConsumedComponents.Contains(component);
        }

        public void Consume(object component)
        {
            ConsumedComponents.Add(component);
        }
    }

    public interface IEntityVisualizationRule
    {
        int Priority { get; }
        void Apply(IEntity entity, EntityVisualizationContext context);
    }
}