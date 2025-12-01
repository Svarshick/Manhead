using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LogicSpace.Fields
{
    public class Map
    {
        public Map(Tilemap tilemap, Dictionary<Vector2Int, Field> fields)
        {
            Tilemap = tilemap;
            Fields = fields;
        }

        public Tilemap Tilemap { get; }
        public int Width => Tilemap.cellBounds.xMax - Tilemap.cellBounds.xMin + 1;
        public int Height => Tilemap.cellBounds.yMax - Tilemap.cellBounds.yMin + 1;
        public Dictionary<Vector2Int, Field> Fields { get; private set; }
        public Vector3 GetCellCenterWorld(Vector3Int position) => Tilemap.GetCellCenterWorld(position);
    }
}