using LogicSpace.EditorData;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(FieldData))]
    public class FieldEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var openButton = new Button(() => EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette"))
            {
                text = "Open Tile Palete",
                style = { height = 40, marginTop = 10 }
            };
            return openButton;
        }
    }
}