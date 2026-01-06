using System;
using System.Collections.Generic;
using LogicSpace.EditorData;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomGridBrush(true, false, false, "Entity Brush")]
    public class EntityBrush : GridBrushBase
    {
        [SerializeField] private BrushCell[] m_Cells;

        [SerializeField] private Vector3Int m_Size;

        [SerializeField] private Vector3Int m_Pivot;

        [SerializeField] [HideInInspector] private bool m_CanChangeZPosition;

        /// <summary>
        ///     Anchor Point of the Instantiated GameObject in the cell when painting
        /// </summary>
        private readonly Vector3 m_Anchor = new(0.5f, 0.5f, 0.0f);

        /// <summary>
        ///     This Brush instances, places and manipulates GameObjects onto the scene.
        /// </summary>
        public EntityBrush()
        {
            Init(Vector3Int.one, Vector3Int.zero);
            SizeUpdated();
        }

        /// <summary>Size of the brush in cells. </summary>
        public Vector3Int size
        {
            get => m_Size;
            set
            {
                m_Size = value;
                SizeUpdated();
            }
        }

        /// <summary>Pivot of the brush. </summary>
        public Vector3Int pivot
        {
            get => m_Pivot;
            set => m_Pivot = value;
        }

        /// <summary>All the brush cells the brush holds. </summary>
        public BrushCell[] cells => m_Cells;

        /// <summary>Number of brush cells in the brush.</summary>
        public int cellCount => m_Cells != null ? m_Cells.Length : 0;

        /// <summary>Number of brush cells based on size.</summary>
        public int sizeCount => m_Size.x * m_Size.y * m_Size.z;

        /// <summary>Whether the brush can change Z Position</summary>
        public bool canChangeZPosition
        {
            get => m_CanChangeZPosition;
            set => m_CanChangeZPosition = value;
        }

        /// <summary>Clears all data of the brush.</summary>
        public void Reset()
        {
            foreach (var cell in m_Cells)
            {
                if (cell.gameObject != null && !EditorUtility.IsPersistent(cell.gameObject))
                    DestroyImmediate(cell.gameObject);
                cell.gameObject = null;
            }

            UpdateSizeAndPivot(Vector3Int.one, Vector3Int.zero);
        }

        private void OnValidate()
        {
            if (m_Size.x < 0)
                m_Size.x = 0;
            if (m_Size.y < 0)
                m_Size.y = 0;
            if (m_Size.z < 0)
                m_Size.z = 0;
        }

        /// <summary>
        ///     Initializes the content of the EntityBrush.
        /// </summary>
        /// <param name="size">Size of the EntityBrush.</param>
        public void Init(Vector3Int size)
        {
            Init(size, Vector3Int.zero);
            SizeUpdated();
        }

        /// <summary>Initializes the content of the EntityBrush.</summary>
        /// <param name="size">Size of the EntityBrush.</param>
        /// <param name="pivot">Pivot point of the EntityBrush.</param>
        public void Init(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            var min = position - pivot;
            var bounds = new BoundsInt(min, m_Size);

            BoxFill(gridLayout, brushTarget, bounds);
        }

        private void PaintCell(GridLayout grid, Vector3Int position, FieldData targetField, BrushCell cell)
        {
            if (cell.gameObject == null)
                return;

            var existingGO = GetObjectInCell(grid, targetField.CellBox.transform, position, m_Anchor, cell.offset);
            if (existingGO == null)
                SetSceneCell(grid, targetField, position, cell.gameObject, cell.offset, cell.scale, cell.orientation,
                    m_Anchor);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            var min = position - pivot;
            var bounds = new BoundsInt(min, m_Size);

            BoxErase(gridLayout, brushTarget, bounds);
        }

        private void EraseCell(GridLayout grid, Vector3Int position, FieldData targetField, BrushCell cell)
        {
            var erased = GetObjectInCell(grid, targetField.CellBox.transform, position, m_Anchor, cell.offset);
            if (erased != null)
                Undo.DestroyObjectImmediate(erased);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (!ValidateTarget(brushTarget, out var targetField))
            {
                Debug.LogWarning("Invalid target");
                return;
            }

            GetGrid(ref gridLayout, ref brushTarget);

            foreach (var location in position.allPositionsWithin)
            {
                var local = location - position.min;
                var cell = m_Cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                PaintCell(gridLayout, location, targetField, cell);
            }
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (!ValidateTarget(brushTarget, out var targetField))
            {
                Debug.LogWarning("Invalid target");
                return;
            }

            GetGrid(ref gridLayout, ref brushTarget);

            foreach (var location in position.allPositionsWithin)
            {
                var local = location - position.min;
                var cell = m_Cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                EraseCell(gridLayout, location, targetField, cell);
            }
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Debug.LogWarning("FloodFill not supported");
        }


        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            var oldSize = m_Size;
            var oldCells = m_Cells.Clone() as BrushCell[];
            size = new Vector3Int(oldSize.y, oldSize.x, oldSize.z);
            var oldBounds = new BoundsInt(Vector3Int.zero, oldSize);

            foreach (var oldPos in oldBounds.allPositionsWithin)
            {
                var newX = direction == RotationDirection.Clockwise ? oldSize.y - oldPos.y - 1 : oldPos.y;
                var newY = direction == RotationDirection.Clockwise ? oldPos.x : oldSize.x - oldPos.x - 1;
                var toIndex = GetCellIndex(newX, newY, oldPos.z);
                var fromIndex = GetCellIndex(oldPos.x, oldPos.y, oldPos.z, oldSize.x, oldSize.y, oldSize.z);
                m_Cells[toIndex] = oldCells[fromIndex];
            }

            var newPivotX = direction == RotationDirection.Clockwise ? oldSize.y - pivot.y - 1 : pivot.y;
            var newPivotY = direction == RotationDirection.Clockwise ? pivot.x : oldSize.x - pivot.x - 1;
            pivot = new Vector3Int(newPivotX, newPivotY, pivot.z);

            var orientation = Quaternion.Euler(0f, 0f, direction != RotationDirection.Clockwise ? 90f : -90f);
            foreach (var cell in m_Cells)
                cell.orientation = cell.orientation * orientation;
        }

        public override void Flip(FlipAxis flip, GridLayout.CellLayout layout)
        {
            if (flip == FlipAxis.X)
                FlipX();
            else
                FlipY();
        }

        /// TODO figure it out
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
        {
            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1),
                new Vector3Int(pivot.x, pivot.y, 0));

            GetGrid(ref gridLayout, ref brushTarget);

            foreach (var pos in position.allPositionsWithin)
            {
                var brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCell(pos, brushPosition, gridLayout, brushTarget != null ? brushTarget.transform : null, true);
            }
        }

        //NOTE: here we pick only Entities
        private void PickCell(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent,
            bool withoutAnchor = false)
        {
            GameObject go = null;
            if (!withoutAnchor)
                go = GetObjectInCell(grid, parent, position, m_Anchor, Vector3.zero);
            if (go == null)
                go = GetObjectInCell(grid, parent, position, Vector3.zero, Vector3.zero);

            var anchorRatio = GetAnchorRatio(grid, m_Anchor);

            var cellLocalPosition = grid.CellToLocalInterpolated(position);
            var anchorLocalPosition = grid.CellToLocalInterpolated(anchorRatio);
            var cellCenter = grid.LocalToWorld(cellLocalPosition + anchorLocalPosition);

            if (go != null && go.GetComponent<EntityData>() != null)
            {
                Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (prefab)
                {
                    SetGameObject(brushPosition, (GameObject)prefab);
                }
                else
                {
                    var newInstance = Instantiate(go);
                    newInstance.hideFlags = HideFlags.HideAndDontSave;
                    newInstance.SetActive(false);
                    SetGameObject(brushPosition, newInstance);
                }

                SetOffset(brushPosition, go.transform.position - cellCenter);
                SetScale(brushPosition, go.transform.localScale);
                SetOrientation(brushPosition, go.transform.localRotation);
            }
        }

        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (!ValidateTarget(brushTarget, out var targetField))
            {
                Debug.LogWarning("Invalid target");
                return;
            }

            GetGrid(ref gridLayout, ref brushTarget);

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), Vector3Int.zero);

            foreach (var pos in position.allPositionsWithin)
            {
                var brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCell(pos, brushPosition, gridLayout, targetField.CellBox.transform);
                var cell = m_Cells[GetCellIndex(brushPosition)];
                EraseCell(gridLayout, pos, targetField, cell);
            }
        }

        /// TODO figure it out
        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (!ValidateTarget(brushTarget, out var targetField))
            {
                Debug.LogWarning("Invalid target");
                return;
            }

            GetGrid(ref gridLayout, ref brushTarget);
            Paint(gridLayout, brushTarget, position.min);
            Reset();
        }

        private void GetGrid(ref GridLayout gridLayout, ref GameObject brushTarget)
        {
            if (brushTarget == null)
            {
                Debug.LogWarning("Target can't be null");
                return;
            }

            gridLayout = brushTarget.GetComponent<GridLayout>();
        }

        //target can't be null
        //NOTE: validating target
        //TODO: erase it in future, because it prevents from painting/erasing on palete
        private bool ValidateTarget(GameObject brushTarget, out FieldData targetField)
        {
            if (brushTarget == null)
            {
                targetField = null;
                return false;
            }

            targetField = brushTarget.GetComponent<FieldData>();
            return targetField != null;
        }

        private void FlipX()
        {
            var oldCells = m_Cells.Clone() as BrushCell[];
            var oldBounds = new BoundsInt(Vector3Int.zero, m_Size);

            foreach (var oldPos in oldBounds.allPositionsWithin)
            {
                var newX = m_Size.x - oldPos.x - 1;
                var toIndex = GetCellIndex(newX, oldPos.y, oldPos.z);
                var fromIndex = GetCellIndex(oldPos);
                m_Cells[toIndex] = oldCells[fromIndex];
            }

            var newPivotX = m_Size.x - pivot.x - 1;
            pivot = new Vector3Int(newPivotX, pivot.y, pivot.z);

            FlipCells(ref m_Cells, new Vector3(-1f, 1f, 1f));
        }

        private void FlipY()
        {
            var oldCells = m_Cells.Clone() as BrushCell[];
            var oldBounds = new BoundsInt(Vector3Int.zero, m_Size);

            foreach (var oldPos in oldBounds.allPositionsWithin)
            {
                var newY = m_Size.y - oldPos.y - 1;
                var toIndex = GetCellIndex(oldPos.x, newY, oldPos.z);
                var fromIndex = GetCellIndex(oldPos);
                m_Cells[toIndex] = oldCells[fromIndex];
            }

            var newPivotY = m_Size.y - pivot.y - 1;
            pivot = new Vector3Int(pivot.x, newPivotY, pivot.z);

            FlipCells(ref m_Cells, new Vector3(1f, -1f, 1f));
        }

        private static void FlipCells(ref BrushCell[] cells, Vector3 scale)
        {
            foreach (var cell in cells) cell.scale = Vector3.Scale(cell.scale, scale);
        }

        /// <summary>Updates the size, pivot and the number of layers of the brush.</summary>
        /// <param name="size">New size of the brush.</param>
        /// <param name="pivot">New pivot of the brush.</param>
        public void UpdateSizeAndPivot(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        /// <summary>
        ///     Sets a GameObject at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the GameObject in the brush.</param>
        /// <param name="go">GameObject to set in the brush.</param>
        public void SetGameObject(Vector3Int position, GameObject go)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].gameObject = go;
        }

        /// <summary>
        ///     Sets a position offset at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the offset in the brush.</param>
        /// <param name="offset">Offset to set in the brush.</param>
        public void SetOffset(Vector3Int position, Vector3 offset)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].offset = offset;
        }

        /// <summary>
        ///     Sets an orientation at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the orientation in the brush.</param>
        /// <param name="orientation">Orientation to set in the brush.</param>
        public void SetOrientation(Vector3Int position, Quaternion orientation)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].orientation = orientation;
        }

        /// <summary>
        ///     Sets a scale at the position in the brush.
        /// </summary>
        /// <param name="position">Position to set the scale in the brush.</param>
        /// <param name="scale">Scale to set in the brush.</param>
        public void SetScale(Vector3Int position, Vector3 scale)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].scale = scale;
        }

        /// <summary>Gets the index to the EntityBrush::ref::BrushCell based on the position of the BrushCell.</summary>
        /// <param name="brushPosition">Position of the BrushCell.</param>
        /// <returns>The cell index for the position of the BrushCell.</returns>
        public int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        public int GetCellIndex(int x, int y, int z)
        {
            return x + m_Size.x * y + m_Size.x * m_Size.y * z;
        }

        public int GetCellIndex(int x, int y, int z, int sizex, int sizey, int sizez)
        {
            return x + sizex * y + sizex * sizey * z;
        }

        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return x % m_Size.x + m_Size.x * (y % m_Size.y) + m_Size.x * m_Size.y * (z % m_Size.z);
        }

        private GameObject GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position, Vector3 anchor,
            Vector3 offset)
        {
            var childCount = parent.childCount;

            var anchorRatio = GetAnchorRatio(grid, anchor);
            var anchorLocal = grid.CellToLocalInterpolated(anchorRatio);
            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                var childCell = grid.LocalToCell(grid.WorldToLocal(child.position) - anchorLocal - offset);
                if (position == childCell)
                    return child.gameObject;
            }

            return null;
        }

        private bool ValidateCellPosition(Vector3Int position)
        {
            var valid =
                position.x >= 0 && position.x < size.x &&
                position.y >= 0 && position.y < size.y &&
                position.z >= 0 && position.z < size.z;
            if (!valid)
                throw new ArgumentException(string.Format(
                    "Position {0} is an invalid cell position. Valid range is between [{1}, {2}).", position,
                    Vector3Int.zero, size));
            return true;
        }

        internal void SizeUpdated(bool keepContents = false)
        {
            OnValidate();
            Array.Resize(ref m_Cells, sizeCount);
            var bounds = new BoundsInt(Vector3Int.zero, m_Size);
            foreach (var pos in bounds.allPositionsWithin)
                if (keepContents || m_Cells[GetCellIndex(pos)] == null)
                    m_Cells[GetCellIndex(pos)] = new BrushCell();
        }

        private static void SetSceneCell(GridLayout grid, FieldData targetField, Vector3Int position, GameObject go,
            Vector3 offset, Vector3 scale, Quaternion orientation, Vector3 anchor)
        {
            if (go == null)
                return;

            GameObject instance;
            if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                instance = (GameObject)PrefabUtility.InstantiatePrefab(go, targetField.transform.root.gameObject.scene);
                instance.transform.parent = targetField.CellBox.transform;
            }
            else
            {
                instance = Instantiate(go, targetField.CellBox.transform);
                instance.name = go.name;
                instance.SetActive(true);
                foreach (var renderer in instance.GetComponentsInChildren<Renderer>()) renderer.enabled = true;
            }

            instance.hideFlags = HideFlags.None;
            Undo.RegisterCreatedObjectUndo(instance, "Paint GameObject");

            var anchorRatio = GetAnchorRatio(grid, anchor);
            instance.transform.position =
                grid.LocalToWorld(grid.CellToLocalInterpolated(position) + grid.CellToLocalInterpolated(anchorRatio));
            instance.transform.localRotation = orientation;
            instance.transform.localScale = scale;
            instance.transform.Translate(offset);
        }

        private static Vector3 GetAnchorRatio(GridLayout grid, Vector3 cellAnchor)
        {
            var cellSize = grid.cellSize;
            var cellStride = cellSize + grid.cellGap;
            cellStride.x = Mathf.Approximately(0f, cellStride.x) ? 1f : cellStride.x;
            cellStride.y = Mathf.Approximately(0f, cellStride.y) ? 1f : cellStride.y;
            cellStride.z = Mathf.Approximately(0f, cellStride.z) ? 1f : cellStride.z;
            var anchorRatio = new Vector3(
                cellAnchor.x * cellSize.x / cellStride.x,
                cellAnchor.y * cellSize.y / cellStride.y,
                cellAnchor.z * cellSize.z / cellStride.z
            );
            return anchorRatio;
        }

        /// <summary>
        ///     Hashes the contents of the brush.
        /// </summary>
        /// <returns>A hash code of the brush</returns>
        public override int GetHashCode()
        {
            var hash = 0;
            unchecked
            {
                foreach (var cell in cells) hash = hash * 33 + cell.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        ///     Brush Cell stores the data to be painted in a grid cell.
        /// </summary>
        [Serializable]
        public class BrushCell
        {
            [SerializeField] private GameObject m_GameObject;

            [SerializeField] private Vector3 m_Offset = Vector3.zero;

            [SerializeField] private Vector3 m_Scale = Vector3.one;

            [SerializeField] private Quaternion m_Orientation = Quaternion.identity;

            /// <summary>
            ///     GameObject to be placed when painting.
            /// </summary>
            public GameObject gameObject
            {
                get => m_GameObject;
                set => m_GameObject = value;
            }

            /// <summary>
            ///     Position offset of the GameObject when painted.
            /// </summary>
            public Vector3 offset
            {
                get => m_Offset;
                set => m_Offset = value;
            }

            /// <summary>
            ///     Scale of the GameObject when painted.
            /// </summary>
            public Vector3 scale
            {
                get => m_Scale;
                set => m_Scale = value;
            }

            /// <summary>
            ///     Orientation of the GameObject when painted.
            /// </summary>
            public Quaternion orientation
            {
                get => m_Orientation;
                set => m_Orientation = value;
            }

            /// <summary>
            ///     Hashes the contents of the brush cell.
            /// </summary>
            /// <returns>A hash code of the brush cell.</returns>
            public override int GetHashCode()
            {
                int hash;
                unchecked
                {
                    hash = gameObject != null ? gameObject.GetInstanceID() : 0;
                    hash = hash * 33 + offset.GetHashCode();
                    hash = hash * 33 + scale.GetHashCode();
                    hash = hash * 33 + orientation.GetHashCode();
                }

                return hash;
            }
        }
    }

    /// <summary>
    ///     The Brush Editor for a GameObject Brush.
    /// </summary>
    [CustomEditor(typeof(EntityBrush))]
    public class EntityBrushEditor : GridBrushEditorBase
    {
        private static readonly string iconPath =
            "Packages/com.unity.2d.tilemap.extras/Editor/Brushes/GameObjectBrush/GameObjectBrush.png";

        private Texture2D m_BrushIcon;

        /// <summary>
        ///     The EntityBrush for this Editor
        /// </summary>
        public EntityBrush brush => target as EntityBrush;

        /// <summary> Whether the GridBrush can change Z Position. </summary>
        public override bool canChangeZPosition
        {
            get => brush.canChangeZPosition;
            set => brush.canChangeZPosition = value;
        }

        /// <summary>
        ///     Whether the Brush is in a state that should be saved for selection.
        /// </summary>
        public override bool shouldSaveBrushForSelection
        {
            get
            {
                if (brush.cells != null)
                    foreach (var cell in brush.cells)
                        if (cell != null && cell.gameObject != null)
                            return true;
                return false;
            }
        }

        /// <summary>
        ///     The targets that the EntityBrush can paint on
        /// </summary>
        /// NOTE: allows to draw only on FieldData
        public override GameObject[] validTargets
        {
            get
            {
                var currentStageHandle = StageUtility.GetCurrentStageHandle();
                var results = currentStageHandle.FindComponentsOfType<FieldData>();
                var validGridLayouts = new List<GameObject>(results.Length);
                foreach (var result in results)
                    if (result.gameObject.scene.isLoaded && result.gameObject.activeInHierarchy)
                        validGridLayouts.Add(result.gameObject);
                return validGridLayouts.ToArray();
            }
        }

        /// <summary> Returns an icon identifying the GameObject Brush. </summary>
        public override Texture2D icon
        {
            get
            {
                if (m_BrushIcon == null)
                {
                    var gui = EditorGUIUtility.TrIconContent(iconPath);
                    m_BrushIcon = gui.image as Texture2D;
                }

                return m_BrushIcon;
            }
        }

        /// <summary>
        ///     Callback for painting the GUI for the GridBrush in the Scene View.
        ///     The EntityBrush Editor overrides this to draw the preview of the brush when drawing lines.
        /// </summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">
        ///     Target of the EntityBrush::ref::Tool operation. By default the currently selected
        ///     GameObject.
        /// </param>
        /// <param name="position">Current selected location of the brush.</param>
        /// <param name="tool">Current EntityBrush::ref::Tool selected.</param>
        /// <param name="executing">Whether brush is being used.</param>
        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position,
            GridBrushBase.Tool tool, bool executing)
        {
            var gizmoRect = position;

            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
                gizmoRect = new BoundsInt(position.min - brush.pivot, brush.size);

            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);
        }

        /// <summary>
        ///     Callback for painting the inspector GUI for the EntityBrush in the tilemap palette.
        ///     The EntityBrush Editor overrides this to show the usage of this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck() && brush.cellCount != brush.sizeCount) brush.SizeUpdated(true);
        }

        /// <summary>
        ///     Creates a static preview of the EntityBrush with its current selection.
        /// </summary>
        /// <param name="assetPath">The asset to operate on.</param>
        /// <param name="subAssets">An array of all Assets at assetPath.</param>
        /// <param name="width">Width of the created texture.</param>
        /// <param name="height">Height of the created texture.</param>
        /// <returns>Generated texture or null.</returns>
        /// TODO could cause problems. It draws on preview, but my drawing requires Target being FieldData
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (brush == null)
                return null;

            var previewInstance = new GameObject("Brush Preview", typeof(Grid));
            var previewGrid = previewInstance.GetComponent<Grid>();

            brush.Paint(previewGrid, previewInstance, Vector3Int.zero);

            var center = (Vector3)brush.size * 0.5f - brush.pivot;
            center.z -= 10f;

            var rect = new Rect(0, 0, width, height);
            var previewUtility = new PreviewRenderUtility(true, true);
            previewUtility.camera.orthographic = true;
            previewUtility.camera.orthographicSize = 1.0f + Math.Max(brush.size.x, brush.size.y);
            if (rect.height > rect.width)
                previewUtility.camera.orthographicSize *= rect.height / rect.width;
            previewUtility.camera.transform.position = center;
            previewUtility.AddSingleGO(previewInstance);
            previewUtility.BeginStaticPreview(rect);
            previewUtility.camera.Render();
            var tex = previewUtility.EndStaticPreview();
            previewUtility.Cleanup();

            DestroyImmediate(previewInstance);

            return tex;
        }
    }
}