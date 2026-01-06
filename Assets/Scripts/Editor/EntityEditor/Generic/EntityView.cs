using LogicSpace.GameEntity;
using PlayerSpace.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.EntityEditor.Generic
{
    public class EntityView : View<EntityViewModel>
    {
        private ComponentsView<EntityComponent> _componentsView;
        private VisualElement _componentsViewRoot;
        private PropertyField _colorProperty;

        public EntityView(EntityViewModel model, VisualElement root) : base(model, root)
        {
        }

        protected override void SetVisualElements()
        {
            _colorProperty = new (ViewModel.ColorSP);
            Root.Add(_colorProperty);
            _componentsViewRoot = new VisualElement();
            Root.Add(_componentsViewRoot);
            _componentsView = new ComponentsView<EntityComponent>(ViewModel.ComponentsViewModel, _componentsViewRoot);
            _componentsView.Initialize();
        }

        protected override void BindViewData()
        {
            _colorProperty.Bind(ViewModel.EntitySO);
            _colorProperty.RegisterValueChangeCallback(_ => ViewModel.dataChangedAction.Invoke());
        }
    }
}