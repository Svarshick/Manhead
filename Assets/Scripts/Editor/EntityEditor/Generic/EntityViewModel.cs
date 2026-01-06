using System;
using LogicSpace.GameEntity;
using PlayerSpace.UI;
using UnityEditor;

namespace Editor.EntityEditor.Generic
{
    public class EntityViewModel : ViewModel
    {
        public readonly ComponentsViewModel<EntityComponent> ComponentsViewModel;
        public readonly SerializedObject EntitySO;
        public readonly SerializedProperty ColorSP;
        public readonly Action dataChangedAction;

        public EntityViewModel(SerializedObject entitySO, Action dataChangedAction)
        {
            EntitySO = entitySO;
            this.dataChangedAction = dataChangedAction;
            var componentsProperty = EntitySO.FindProperty("Components");
            ComponentsViewModel =
                new ComponentsViewModel<EntityComponent>(EntitySO, componentsProperty, this.dataChangedAction);
            ColorSP = EntitySO.FindProperty("Color");
        }

        public void Update()
        {
            ComponentsViewModel.Update(true);
        }
    }
}