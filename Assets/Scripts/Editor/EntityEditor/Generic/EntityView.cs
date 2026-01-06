using LogicSpace.GameEntity;
using PlayerSpace.UI;
using UnityEngine.UIElements;

namespace Editor.EntityEditor.Generic
{
    public class EntityView : View<EntityViewModel>
    {
        private ComponentsView<EntityComponent> _componentsView;
        private VisualElement _componentsViewRoot;

        public EntityView(EntityViewModel model, VisualElement root) : base(model, root)
        {
        }

        protected override void SetVisualElements()
        {
            _componentsViewRoot = new VisualElement();
            Root.Add(_componentsViewRoot);
            _componentsView = new ComponentsView<EntityComponent>(ViewModel.ComponentsViewModel, _componentsViewRoot);
            _componentsView.Initialize();
        }
    }
}