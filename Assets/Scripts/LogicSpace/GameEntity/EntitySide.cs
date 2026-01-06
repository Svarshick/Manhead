using System;
using System.Collections.Generic;
using CustomMath;
using UnityEngine;

namespace LogicSpace.GameEntity
{
    public class EntitySide : IEntitySide
    {
        private readonly Dictionary<Type, EntitySideComponent> _components;
        public readonly Entity Entity;
        public readonly Direction LocalSideDirection;

        public EntitySide(Entity entity, Direction localSideDirection)
        {
            Entity = entity;
            LocalSideDirection = localSideDirection;
            _components = new Dictionary<Type, EntitySideComponent>();
        }

        public EntitySide(Entity entity, Direction localSideDirection, List<EntitySideComponent> components)
        {
            Entity = entity;
            LocalSideDirection = localSideDirection;
            _components = new Dictionary<Type, EntitySideComponent>();
            foreach (var component in components)
            {
                var type = component.GetType();
                if (_components.ContainsKey(type))
                {
                    Debug.LogAssertion($"EntitySide of {entity} get more than one component of type {type}");
                    _components[type] = component;
                }
                else
                {
                    _components.Add(component.GetType(), component);
                }
            }
        }

        public Direction GlobalSideDirection =>
            SpaceUtility.GetGlobalDirection(LocalSideDirection, Entity.LookDirection);

        public T GetComponent<T>() where T : EntitySideComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
                return (T)component;
            return default;
        }

        public T AddComponent<T>(T component) where T : EntitySideComponent, new()
        {
            if (_components.ContainsKey(typeof(T)))
                //TODO make exception better?
                throw new Exception("Component is already added");
            _components.Add(typeof(T), component);
            return component;
        }
    }
}