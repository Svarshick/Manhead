using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LogicSpace
{
    public class Init : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        private FieldsGrid _fieldsGrid; 

        void Start()
        {
            tilemap.CompressBounds();
            int width = tilemap.cellBounds.xMax - tilemap.cellBounds.xMin + 1;
            int height = tilemap.cellBounds.yMax - tilemap.cellBounds.yMin + 1;
            Dictionary<Vector2Int, Field> fields = new(width * height);
            _fieldsGrid = new(tilemap, fields);
            for (int x = tilemap.cellBounds.xMin - 10; x <= tilemap.cellBounds.xMax + 10; ++x)
            {
                for (int y = tilemap.cellBounds.yMin - 10; y <= tilemap.cellBounds.yMax + 10; ++y)
                {
                    var position = new Vector2Int(x, y);
                    _fieldsGrid.Fields[position] = new Field(_fieldsGrid, position);
                }
            }

            foreach (var cell in tilemap.GetComponentsInChildren<Cell>())
            {
                var cellPosition = (Vector2Int) _fieldsGrid.Tilemap.WorldToCell(cell.transform.position);
                _fieldsGrid.Fields[cellPosition].Cells.Add(cell);
                cell.ChangeField(_fieldsGrid.Fields[cellPosition]);
            }
        }
    }
}