using System.Collections.Generic;
using LogicSpace.Cell;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LogicSpace
{
    public static class MapFabric
    {
        public static Map CreateFromTilemap(Tilemap tilemap)
        {
            tilemap.CompressBounds();
            int padding = 5;
            int xMin = tilemap.cellBounds.xMin - padding;
            int yMin = tilemap.cellBounds.yMin - padding;
            int xMax = tilemap.cellBounds.xMax + padding;
            int yMax = tilemap.cellBounds.yMax + padding;
            int width = xMax - xMin + 1;
            int height = yMax - yMin + 1;
            Dictionary<Vector2Int, Field> fields = new(width * height);
            var fieldsGrid = new Map(tilemap, fields);
            for (int x = xMin; x <= xMax; ++x)
            {
                for (int y = yMin; y <= yMax; ++y)
                {
                    var position = new Vector2Int(x, y);
                    fieldsGrid.Fields[position] = new Field(fieldsGrid, position);
                }
            }

            foreach (var cell in tilemap.GetComponentsInChildren<Cell.Cell>())
            {
                var cellPosition = (Vector2Int) fieldsGrid.Tilemap.WorldToCell(cell.transform.position);
                fieldsGrid.Fields[cellPosition].Cells.Add(cell);
                cell.ChangeField(fieldsGrid.Fields[cellPosition]);
            }
            
            return fieldsGrid;
        }
    }
}