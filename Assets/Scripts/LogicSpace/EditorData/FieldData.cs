using UnityEngine;

namespace LogicSpace.EditorData
{
    [RequireComponent(typeof(Grid))]
    public class FieldData : MonoBehaviour
    {
        [field: SerializeField] [field: HideInInspector]
        private Grid _grid;

        [SerializeField] [HideInInspector] private GameObject _cellBox;

        public Grid Grid
        {
            get
            {
                if (_grid != null)
                    return _grid;
                _grid = GetComponentInParent<Grid>();
                return _grid;
            }
        }

        public GameObject CellBox
        {
            get
            {
                if (_cellBox != null)
                    return _cellBox;
                _cellBox = new GameObject("CellBox");
                _cellBox.transform.parent = transform;
                return _cellBox;
            }
        }
    }
}