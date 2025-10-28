using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LogicSpace
{
    public class FieldsGrid
    {
        public Tilemap Tilemap { get; private set; }
        public int Width => Tilemap.cellBounds.xMax - Tilemap.cellBounds.xMin + 1;
        public int Height => Tilemap.cellBounds.yMax - Tilemap.cellBounds.yMin + 1;
        public Dictionary<Vector2Int, Field> Fields { get; private set; } = new();
        
        public FieldsGrid(Tilemap tilemap, Dictionary<Vector2Int, Field> fields)
        {
            Tilemap = tilemap;
            Fields = fields;
        }
        
        public Vector3 GetCellCenterWorld(Vector3Int position) => Tilemap.GetCellCenterWorld(position);
    }
}