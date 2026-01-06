using System.Collections.Generic;
using CustomMath;
using LogicSpace.EditorData;
using LogicSpace.EntityAppearance;
using LogicSpace.EntityAppearance.Rules;
using LogicSpace.GameEntity;
using UnityEngine;

namespace LogicSpace.GameField
{
    public class FieldFactory
    {
        public static Field CreateFromData(FieldData fieldData)
        {
            var field = new Field(fieldData.Grid);
            var (fieldCells, fieldBounds) = ExtractCells(fieldData, field);

            field.SetCells(fieldCells, fieldBounds);
            return field;
        }

        public static Field CreateFromDataAndSpawnCrossroads(FieldData fieldData, int crossroadsAmount)
        {
            var field = new Field(fieldData.Grid);
            var (fieldCells, fieldBounds) = ExtractCells(fieldData, field);

            var emptyCells = new List<Cell>();
            foreach (var cell in fieldCells.Values)
                if (cell.Entities.Count == 0)
                    emptyCells.Add(cell);
            if (emptyCells.Count < crossroadsAmount)
                crossroadsAmount = emptyCells.Count;
            var arrowRule = new ArrowEntityVisualizationRule();
            var baseRule = new BaseEntityVisualizationRule();
            var appearanceBuilder = new EntityAppearanceBuilder(new IEntityVisualizationRule[] { arrowRule, baseRule });
            for (var i = 0; i < crossroadsAmount; i++)
            {
                var n = Random.Range(0, emptyCells.Count);
                var crossroad = CreateRandomCrossroad();
                crossroad.ChangeCell(emptyCells[n]);
                crossroad.Appearance = appearanceBuilder.BuildAppearance(crossroad);
                emptyCells.RemoveAt(n);
            }


            field.SetCells(fieldCells, fieldBounds);
            return field;
        }

        private static (Dictionary<Vector2Int, Cell>, BoundsInt) ExtractCells(FieldData fieldData, Field field)
        {
            var fieldGrid = fieldData.Grid;

            var xMax = int.MaxValue;
            var yMax = int.MaxValue;
            var xMin = int.MinValue;
            var yMin = int.MinValue;
            var hasBounds = false;
            foreach (var entityData in fieldData.CellBox.GetComponentsInChildren<EntityData>())
            {
                var cellPosition = fieldGrid.WorldToCell(entityData.transform.position);
                if (!hasBounds)
                {
                    xMin = cellPosition.x;
                    xMax = cellPosition.x;
                    yMin = cellPosition.y;
                    yMax = cellPosition.y;
                    hasBounds = true;
                }
                else
                {
                    xMin = Mathf.Min(xMin, cellPosition.x);
                    xMax = Mathf.Max(xMax, cellPosition.x);
                    yMin = Mathf.Min(yMin, cellPosition.y);
                    yMax = Mathf.Max(yMax, cellPosition.y);
                }
            }

            var min = new Vector3Int(xMin, yMin, 0);
            var max = new Vector3Int(xMax, yMax, 0);
            var fieldBounds = new BoundsInt(min, max - min);

            var width = xMax - xMin + 1;
            var height = yMax - yMin + 1;
            var fieldCells = new Dictionary<Vector2Int, Cell>(width * height);
            for (var x = xMin; x <= xMax; x++)
            for (var y = yMin; y <= yMax; y++)
            {
                var position = new Vector2Int(x, y);
                fieldCells[position] = new Cell(field, position);
            }

            foreach (var entityData in fieldData.CellBox.GetComponentsInChildren<EntityData>())
            {
                var cellPosition = (Vector2Int)fieldGrid.WorldToCell(entityData.transform.position);
                var entity = new Entity(entityData);
                entity.ChangeCell(fieldCells[cellPosition]);
            }

            return (fieldCells, fieldBounds);
        }

        private static Entity CreateRandomCrossroad()
        {
            var entity = new Entity();
            var sidesMask = Random.Range(1, 16);
            for (var sideDirection = 0; sideDirection < 3; sideDirection++)
                if ((sidesMask & (int)Mathf.Pow(2, sideDirection)) != 0)
                {
                    var side = entity.GetSide((Direction)sideDirection);
                    var crossroad = new Crossroad();
                    crossroad.rotationDirection = DirectionUtils.GetRandom();
                    side.AddComponent(crossroad);
                }

            return entity;
        }
    }
}