using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;

namespace LogicSpace
{
    public class MapDebugger : MonoBehaviour
    {
        private Map _map;
        private MapDebugView _mapDebugView;
        
        private LineRenderer _borderLineRenderer;
        private LineRenderer _fieldLineRenderer;

        private InputAction _clickAction;
        
        private Field _currentField;

        [Inject]
        public void Inject(Map map, MapDebugView mapDebugView)
        {
            _map = map;
            _mapDebugView = mapDebugView;
            _mapDebugView.Initialize();
        }
        
        void Awake()
        {
            InitBorderLineRenderer();
            InitFieldLineRenderer();
            _clickAction = InputSystem.actions["Click"];
            

            void InitBorderLineRenderer()
            {
                var borderRendererGO = new GameObject("Border Renderer");
                borderRendererGO.transform.SetParent(transform);
                _borderLineRenderer = borderRendererGO.AddComponent<LineRenderer>();
                _borderLineRenderer.positionCount = 4;
                _borderLineRenderer.loop = true;
                _borderLineRenderer.startWidth = .05f;
                _borderLineRenderer.endWidth = .05f;
                _borderLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                _borderLineRenderer.startColor = Color.black;
                _borderLineRenderer.endColor = Color.black;    
            }
            void InitFieldLineRenderer()
            {
                var fieldRendererGO = new GameObject("Field Renderer");
                fieldRendererGO.transform.SetParent(transform);
                _fieldLineRenderer = fieldRendererGO.AddComponent<LineRenderer>();
                _fieldLineRenderer.positionCount = 4;
                _fieldLineRenderer.loop = true;
                _fieldLineRenderer.startWidth = .05f;
                _fieldLineRenderer.endWidth = .05f;
                _fieldLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                _fieldLineRenderer.startColor = Color.red;
                _fieldLineRenderer.endColor = Color.red;

            }
        }

        void Update()
        {
            if (_map == null || _mapDebugView == null)
                return;
            
            if (_clickAction.triggered)
            {
                DrawBorder();

                var pointerPosition = Pointer.current.position.ReadValue();
                var clickWorldPosition = Camera.main.ScreenToWorldPoint(pointerPosition);
                var fieldGridPosition = _map.Tilemap.WorldToCell(clickWorldPosition);
                var fieldWorldPosition = _map.Tilemap.CellToWorld(fieldGridPosition);
                if (_map.Fields.TryGetValue((Vector2Int) fieldGridPosition, out _currentField))
                {
                    _fieldLineRenderer.enabled = true;
                    ShowFieldDebug(fieldWorldPosition);
                    DrawField(fieldWorldPosition);
                }
                else
                {
                    _fieldLineRenderer.enabled = false;
                    _mapDebugView.HideFieldDebug();
                    _currentField = null;
                }
            }

            if (_currentField != null)
            {
                string info = $"Field position: {_currentField.GridPosition.ToString()}\n\n";
                foreach (var cell in _currentField.Cells)
                {
                    string cellInfo = $"Cell: {cell.gameObject}\n";
                    info += cellInfo + '\n';
                }
                _mapDebugView.SetInfo(info);
            }
        }

        private void DrawBorder()
        {
            var padding = 5;
            var bounds = _map.Tilemap.cellBounds;

            // Calculate world positions for the four corners
            Vector3[] corners = new Vector3[5];
    
            // Bottom-left
            corners[0] = _map.Tilemap.CellToWorld(new Vector3Int(bounds.xMin - padding, bounds.yMin - padding, 0));
            // Bottom-right
            corners[1] = _map.Tilemap.CellToWorld(new Vector3Int(bounds.xMax + padding, bounds.yMin - padding, 0));
            // Top-right
            corners[2] = _map.Tilemap.CellToWorld(new Vector3Int(bounds.xMax + padding, bounds.yMax + padding, 0));
            // Top-left
            corners[3] = _map.Tilemap.CellToWorld(new Vector3Int(bounds.xMin - padding, bounds.yMax + padding, 0));
            // Back to start to close the loop
            corners[4] = corners[0];

            // Adjust for cell center (if your tiles are centered)
            var cellCenterOffset = _map.Tilemap.cellSize * 0.5f;
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] += cellCenterOffset;
            }

            // Set positions in LineRenderer
            _borderLineRenderer.positionCount = corners.Length;
            _borderLineRenderer.SetPositions(corners);
        }

        private void DrawField(Vector3 fieldWorldPosition)
        {
            var cellCenter =  fieldWorldPosition + _map.Tilemap.cellSize * 0.5f;
    
            // Calculate the four corners of the cell
            var halfSize = _map.Tilemap.cellSize * 0.5f;
    
            Vector3[] corners = new Vector3[5];
            corners[0] = cellCenter + new Vector3(-halfSize.x, -halfSize.y, 0); // Bottom-left
            corners[1] = cellCenter + new Vector3(halfSize.x, -halfSize.y, 0);  // Bottom-right
            corners[2] = cellCenter + new Vector3(halfSize.x, halfSize.y, 0);   // Top-right
            corners[3] = cellCenter + new Vector3(-halfSize.x, halfSize.y, 0);  // Top-left
            corners[4] = corners[0]; // Close the loop

            _fieldLineRenderer.positionCount = corners.Length;
            _fieldLineRenderer.SetPositions(corners);
            _fieldLineRenderer.gameObject.SetActive(true); 
        }

        private void ShowFieldDebug(Vector3 fieldWorldPosition)
        {
            _mapDebugView.ShowFieldDebug(fieldWorldPosition);
        }
    }
}