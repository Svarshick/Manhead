using System;
using System.Collections.Generic;
using LogicSpace.Cells;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LogicSpace.Fields
{
    public static class MapFactory
    {
        public static Map CreateFromTilemap(Tilemap tilemap)
        {
            var xMax=int.MaxValue;
            var yMax=int.MaxValue;
            var xMin=int.MinValue;
            var yMin=int.MinValue;
            var hasBounds = false;
            
            foreach (var cell in tilemap.GetComponentsInChildren<Cell>())
            {
                var cellPosition = tilemap.WorldToCell(cell.transform.position);
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
                    xMin = Math.Min(xMin, cellPosition.x);
                    xMax = Math.Max(xMax, cellPosition.x);
                    yMin = Math.Min(yMin, cellPosition.y);
                    yMax = Math.Max(yMax, cellPosition.y);
                }
            }

            var width = xMax - xMin + 1;
            var height = yMax - yMin + 1;
            Dictionary<Vector2Int, Field> fields = new(width * height);
            var bounds = new BoundsInt();
            bounds.SetMinMax(new Vector3Int(xMin, yMin, 0), new Vector3Int(xMax, yMax, 0));
            var map = new Map(tilemap, fields, bounds); 
            
            for (var x = xMin; x <= xMax; ++x)
                for (var y = yMin; y <= yMax; ++y)
                {
                    var position = new Vector2Int(x, y);
                    map.Fields[position] = new Field(map, position);
                }

            foreach (var cell in tilemap.GetComponentsInChildren<Cell>())
            {
                var cellPosition = (Vector2Int)map.Tilemap.WorldToCell(cell.transform.position);
                CellInitializer.Init(cell, map.Fields[cellPosition]);
            }

            return map;
        }

        public static Map CreateFromTilemapWithCrossroadsGeneration(Tilemap tilemap, int crossroadsAmount)
        {
            var crossroadAsset = Resources.Load<GameObject>("Crossroad");
            
            var map = CreateFromTilemap(tilemap);
            var emptyFields = new List<Field>();
            foreach (var field in map.Fields.Values)
            {
                if (field.Cells.Count == 0)
                    emptyFields.Add(field);
            }
            if (emptyFields.Count < crossroadsAmount)
                crossroadsAmount = emptyFields.Count;
            for (int i = 0; i < crossroadsAmount; ++i)
            {
                var n = Random.Range(0, emptyFields.Count);
                var field = emptyFields[n];
                var crossroad = Object.Instantiate(crossroadAsset, tilemap.transform);
                crossroad.transform.position = field.WorldPosition;
                var cell =  crossroad.GetComponent<Cell>();
                CellInitializer.Init(cell, field);
                cell.LeftSide.GetComponent<CrossRoad>().LocalRotationDirection = DirectionUtils.GetRandom();
                cell.RightSide.GetComponent<CrossRoad>().LocalRotationDirection = DirectionUtils.GetRandom();
                cell.FrontSide.GetComponent<CrossRoad>().LocalRotationDirection = DirectionUtils.GetRandom();
                cell.BackSide.GetComponent<CrossRoad>().LocalRotationDirection = DirectionUtils.GetRandom();
                field.Cells.Add(cell);
                emptyFields.RemoveAt(n);
            }
            return map;
        }
    }

}