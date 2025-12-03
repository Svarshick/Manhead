using System;
using System.Collections.Generic;
using LogicSpace.Fields;
using UnityEngine;

namespace LogicSpace.Cells
{

    public static class CellInitializer
    {
        public static void Init(Cell cell, Field field)
        {
            var cellGO = cell.gameObject;
            var components = new Dictionary<Type, CellComponent>(cellGO.GetComponentCount());
            foreach (var component in cellGO.GetComponents<CellComponent>())
            {
                var componentType = component.GetType();
                if (components.ContainsKey(componentType))
                    Debug.LogAssertion($"{cell} contains more than one component of type {componentType}");
                components[component.GetType()] = component;
            }
            cell.Components = components;
            InitCellSide(cell, Direction.Left);
            InitCellSide(cell, Direction.Right);
            InitCellSide(cell, Direction.Up);
            InitCellSide(cell, Direction.Down);
            cell.ChangeField(field);
            cellGO.SetActive(true);
        }

        private static void InitCellSide(Cell cell, Direction side)
        {
            var cellSide = cell.GetSide(side);
            var cellSideGO = cellSide.gameObject;
            var components = new Dictionary<Type, CellSideComponent>(cellSideGO.GetComponentCount());
            foreach (var component in cellSideGO.GetComponents<CellSideComponent>())
            {
                var componentType = component.GetType();
                if (components.ContainsKey(componentType))
                    Debug.LogAssertion($"{cellSide} contains more than one component of type {componentType}");
                components[component.GetType()] = component;
            }
            cellSide.Components = components;
            cellSide.Cell = cell;
            cellSide.Side = side;
            cellSideGO.SetActive(true);
        }
    }
}