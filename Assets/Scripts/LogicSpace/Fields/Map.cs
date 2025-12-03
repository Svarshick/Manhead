using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LogicSpace.Fields
{
    public class Map
    {
        public Map(Tilemap tilemap, Dictionary<Vector2Int, Field> fields, BoundsInt bounds)
        {
            Tilemap = tilemap;
            Fields = fields;
            Bounds = bounds;
        }

        public BoundsInt Bounds;
        public int Width => Bounds.max.x - Bounds.min.x + 1;
        public int Height => Bounds.max.y - Bounds.min.y + 1;
        public Vector3 WorldMin => new Vector3(Bounds.min.x * Tilemap.cellSize.x, Bounds.min.y * Tilemap.cellSize.y);
        public Vector3 WorldMax => new Vector3((Bounds.max.x + 1)* Tilemap.cellSize.x, (Bounds.max.y + 1) * Tilemap.cellSize.y);
        public float FieldWidth => Tilemap.cellSize.x;
        public float FieldHeight => Tilemap.cellSize.y;
        public Tilemap Tilemap { get; }
        public Dictionary<Vector2Int, Field> Fields { get; private set; }
        public Vector3 GetCellCenterWorld(Vector3Int position) => Tilemap.GetCellCenterWorld(position);
    }
}