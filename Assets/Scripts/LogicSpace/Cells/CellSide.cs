using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicSpace.Cells
{
    public class CellSide : MonoBehaviour
    {
        public Cell Cell { get; set; }
        public Dictionary<Type, CellSideComponent> Components { get; private set; }

        void Awake()
        {
            Components = new (gameObject.GetComponentCount());
            foreach (var component in GetComponents<CellSideComponent>())
            {
                var componentType = component.GetType();
                if (Components.ContainsKey(componentType))
                    Debug.LogAssertion($"{this} contains more than one component of type {componentType}");
                Components[component.GetType()] = component;
            }
        }
    }

    [RequireComponent(typeof(CellSide))]
    public class CellSideComponent : MonoBehaviour
    {
        public CellSide CellSide { get; private set; }

        void Awake()
        {
            CellSide = GetComponent<CellSide>();
        }
    }
}