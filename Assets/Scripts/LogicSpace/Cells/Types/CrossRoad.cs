using UnityEngine;
using UnityEngine.Serialization;

namespace LogicSpace.Cells
{
    public class CrossRoad : CellSideComponent
    {
        [field: SerializeField] public Color Color { get; set; } = Color.black;
        [field: FormerlySerializedAs("RotationDirection")] [field: SerializeField] public Direction LocalRotationDirection { get; set; }
        public Direction GlobalRotationDirection =>
            MyMath.TurnVector(LocalRotationDirection.ToVector2Int(), CellSide.Cell.LookDirection).ToDirection();

        private LineRenderer _directionRenderer;

        void Awake()
        {
            base.Awake();
            var directionRendererGO = new GameObject("Direction Renderer");
            directionRendererGO.transform.SetParent(transform);
            _directionRenderer = directionRendererGO.AddComponent<LineRenderer>();
            _directionRenderer.positionCount = 5;
            _directionRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _directionRenderer.material.color = Color;
            _directionRenderer.startWidth = .05f;
            _directionRenderer.endWidth = .05f;
        }

        void Update()
        {
            var lookVector = CellSide.GetLookVector();
            var width = CellSide.Cell.Field.Map.FieldWidth;
            var height = CellSide.Cell.Field.Map.FieldHeight;
            var position = CellSide.Cell.Field.WorldPosition;
            var arrowPoints = new Vector3[5];
            arrowPoints[0] = Vector2.left;
            arrowPoints[1] = Vector2.up;
            arrowPoints[2] = Vector2.down;
            arrowPoints[3] = Vector2.up;
            arrowPoints[4] = Vector2.right;
            
            for(int i = 0; i < 5; i++)
            {
                arrowPoints[i] = MyMath.TurnVector(arrowPoints[i], GlobalRotationDirection);
            }
            Vector3 arrowShift = position;
            arrowShift.x += lookVector.x * width / 3;
            arrowShift.y += lookVector.y * height / 3;
            for (int i = 0; i < 5; i++)
            {
                var point = arrowPoints[i];
                point.x *= width / 6;
                point.y *= height / 6;
                point += arrowShift;
                arrowPoints[i] = point;
            }
            _directionRenderer.SetPositions(arrowPoints);
        }
    }
}