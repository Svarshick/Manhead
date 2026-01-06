using System;
using LogicSpace.GameEntity;
using PlayerSpace.UI;
using UnityEditor;

namespace Editor.EntityEditor.Generic
{
    public class EntityViewModel : ViewModel
    {
        public ComponentsViewModel<EntityComponent> ComponentsViewModel;
        public SerializedObject EntitySO;

        public EntityViewModel(SerializedObject entitySo, Action dataChangedAction)
        {
            EntitySO = entitySo;
            var componentsProperty = EntitySO.FindProperty("Components");
            ComponentsViewModel =
                new ComponentsViewModel<EntityComponent>(EntitySO, componentsProperty, dataChangedAction);
        }

        public void Update()
        {
            ComponentsViewModel.Update(true);
        }
    }
}