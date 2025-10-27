using UnityEngine;

namespace LogicSpace
{
    public class Cell : MonoBehaviour
    {
        public Field Field { get; private set; }

        [field: SerializeField]
        public float Speed { get; set; } = 1f;

        public void ChangeField(Field field)
        {
            Field?.Cells.Remove(this);
            Field = field ?? throw new System.ArgumentNullException(nameof(field));
            field.Cells.Add(this);
        }
    }
}