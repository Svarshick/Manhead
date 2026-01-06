using LogicSpace.GameEntity;
using PlayerSpace.UI;
using UnityEngine.UIElements;

namespace Editor.EntityEditor.Generic
{
    public class EntitySideView : View<EntitySideViewModel>
    {
        private ComponentsView<EntitySideComponent> _componentsView;
        private VisualElement _componentsViewRoot;

        public EntitySideView(EntitySideViewModel model, VisualElement root) : base(model, root)
        {
        }

        protected override void SetVisualElements()
        {
            _componentsViewRoot = new VisualElement();
            Root.Add(_componentsViewRoot);
            _componentsView =
                new ComponentsView<EntitySideComponent>(ViewModel.ComponentsViewModel, _componentsViewRoot);
            _componentsView.Initialize();
        }
    }
}