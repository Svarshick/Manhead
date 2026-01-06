using System.Collections.Generic;
using System.Linq;
using LogicSpace.GameEntity;
using UnityEngine;

namespace LogicSpace.EntityAppearance
{
    public class EntityAppearanceBuilder
    {
        private readonly IEntityVisualizationRule[] _rules;

        public EntityAppearanceBuilder(IEnumerable<IEntityVisualizationRule> rules)
        {
            _rules = rules.OrderByDescending(vr => vr.Priority).ToArray();
        }

        public GameObject BuildAppearance(IEntity entity)
        {
            var rootGO = new GameObject("Appearance");
            //rootGO.hideFlags = HideFlags.HideAndDontSave; //if something went wrong
            var context = new EntityVisualizationContext { RootGO = rootGO };
            foreach (var rule in _rules) rule.Apply(entity, context);

            return rootGO;
        }
    }
}