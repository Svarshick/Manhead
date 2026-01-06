using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PlayerSpace.UI
{
    public static class UIUtils
    {
        public static VisualElement CreateAutoInspector(SerializedObject so)
        {
            var root = new VisualElement();
            // Use GetIterator to iterate through the ScriptableObject's fields
            var iterator = so.GetIterator();

            if (iterator.NextVisible(true))
                do
                {
                    if (iterator.name == "m_Script") continue; // Skip the default Script field

                    var field = new PropertyField(iterator);
                    field.Bind(so); // Binds the UI field to the SerializedObject
                    root.Add(field);
                } while (iterator.NextVisible(false));

            return root;
        }
    }
}