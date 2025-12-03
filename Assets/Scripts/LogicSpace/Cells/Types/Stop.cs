using UnityEngine;

namespace LogicSpace.Cells
{
    public class Stop : CellSideComponent
    {
        private LineRenderer _sideRenderer;
        void Awake()
        {
            base.Awake();
            var sideRendererGO = new GameObject("Side Renderer");
            sideRendererGO.transform.SetParent(transform);
            _sideRenderer = sideRendererGO.AddComponent<LineRenderer>();
            _sideRenderer.positionCount = 2;
            _sideRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _sideRenderer.material.color = Color.black;
            _sideRenderer.startWidth = .05f;
            _sideRenderer.endWidth = .05f;
        }

        void Update()
        {
            var lookVector = CellSide.GetLookVector();
            //TODO is it awful? (lots of Class.Link.To.Class)
            var xShift = CellSide.Cell.Field.Map.FieldWidth / 2;
            var yShift = CellSide.Cell.Field.Map.FieldHeight / 2;
            var position = CellSide.Cell.Field.WorldPosition;
            Vector3 point0;
            Vector3 point1;
            if (lookVector.x != 0)
            {
                var xPos = position.x + lookVector.x * xShift;
                point0 = new Vector3(xPos, position.y - yShift);
                point1 = new Vector3(xPos, position.y + yShift);
            }
            else
            {
                var yPos = position.y + lookVector.y * yShift;
                point0 = new Vector3(position.x - xShift, yPos);
                point1 = new Vector3(position.x + xShift, yPos);
            }

            _sideRenderer.SetPosition(0, point0);
            _sideRenderer.SetPosition(1, point1);
        }
    }
}