using System;
using LogicSpace.GameEntity;
using PlayerSpace.UI;
using UnityEditor;

namespace Editor.EntityEditor.Generic
{
    public class EntitySideViewModel : ViewModel
    {
        public readonly ComponentsViewModel<EntitySideComponent> ComponentsViewModel;
        public readonly SerializedProperty SerializedProperty;
        public readonly SerializedObject TargetSO;

        public EntitySideViewModel(
            SerializedObject targetSO,
            SerializedProperty serializedProperty,
            Action dataChangedAction)
        {
            TargetSO = targetSO;
            SerializedProperty = serializedProperty;
            var componentsProperty = SerializedProperty.FindPropertyRelative("Components");
            ComponentsViewModel =
                new ComponentsViewModel<EntitySideComponent>(TargetSO, componentsProperty, dataChangedAction);
        }

        public void Update()
        {
            ComponentsViewModel.Update(true);
        }
    }
}