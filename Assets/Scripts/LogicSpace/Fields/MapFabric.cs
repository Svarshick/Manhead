using System.Collections.Generic;
using LogicSpace.Cells;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LogicSpace.Fields
{
    public static class MapFabric
    {
        public static Map CreateFromTilemap(Tilemap tilemap)
        {
            tilemap.CompressBounds();
            var padding = 5;
            var xMin = tilemap.cellBounds.xMin - padding;
            var yMin = tilemap.cellBounds.yMin - padding;
            var xMax = tilemap.cellBounds.xMax + padding;
            var yMax = tilemap.cellBounds.yMax + padding;
            var width = xMax - xMin + 1;
            var height = yMax - yMin + 1;
            Dictionary<Vector2Int, Field> fields = new(width * height);
            var fieldsGrid = new Map(tilemap, fields);
            for (var x = xMin; x <= xMax; ++x)
            for (var y = yMin; y <= yMax; ++y)
            {
                var position = new Vector2Int(x, y);
                fieldsGrid.Fields[position] = new Field(fieldsGrid, position);
            }

            foreach (var cell in tilemap.GetComponentsInChildren<Cell>())
            {
                var cellPosition = (Vector2Int)fieldsGrid.Tilemap.WorldToCell(cell.transform.position);
                fieldsGrid.Fields[cellPosition].Cells.Add(cell);
                cell.ChangeField(fieldsGrid.Fields[cellPosition]);
            }

            return fieldsGrid;
        }
    }
}