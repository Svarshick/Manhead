using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LogicSpace
{
    public class FieldsGrid
    {
        public Tilemap Tilemap { get; private set; }
        public Dictionary<Vector2Int, Field> Fields { get; private set; } = new();

        public FieldsGrid(Tilemap tilemap, Dictionary<Vector2Int, Field> fields)
        {
            Tilemap = tilemap;
            Fields = fields;
        }
        
        public Vector3 GetCellCenterWorld(Vector3Int position) => Tilemap.GetCellCenterWorld(position);
    }
}