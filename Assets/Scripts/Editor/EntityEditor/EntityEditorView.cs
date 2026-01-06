using Editor.EntityEditor.Generic;
using PlayerSpace.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.EntityEditor
{
    public class EntityEditorView : View<EntityEditorViewModel>
    {
        private Tab _entityBackSideTab;
        private Tab _entityFrontSideTab;
        private Tab _entityLeftSideTab;
        private Tab _entityRightSideTab;
        private Tab _entityTab;
        private TabView _tabView;

        public EntityEditorView(EntityEditorViewModel model, VisualElement root) : base(model, root)
        {
        }

        protected override void SetVisualElements()
        {
            _tabView = new TabView();

            _entityTab = new Tab("Self");
            _entityFrontSideTab = new Tab("Front");
            _entityLeftSideTab = new Tab("Left");
            _entityBackSideTab = new Tab("Back");
            _entityRightSideTab = new Tab("Right");
            var entityView = new EntityView(ViewModel.EntityViewModel, _entityTab);
            var entityFrontSideView = new EntitySideView(ViewModel.FrontSideViewModel, _entityFrontSideTab);
            var entityLeftSideView = new EntitySideView(ViewModel.LeftSideViewModel, _entityLeftSideTab);
            var entityBackSideView = new EntitySideView(ViewModel.BackSideViewModel, _entityBackSideTab);
            var entityRightSideView = new EntitySideView(ViewModel.RightSideViewModel, _entityRightSideTab);
            entityView.Initialize();
            entityFrontSideView.Initialize();
            entityLeftSideView.Initialize();
            entityBackSideView.Initialize();
            entityRightSideView.Initialize();
            _tabView.Add(_entityTab);
            _tabView.Add(_entityFrontSideTab);
            _tabView.Add(_entityLeftSideTab);
            _tabView.Add(_entityBackSideTab);
            _tabView.Add(_entityRightSideTab);

            Root.Add(_tabView);

            //gemini generated
            _tabView.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                var header = _tabView.Q<VisualElement>(className: "unity-tab-view__header-container");
                if (header != null)
                {
                    header.style.width = Length.Percent(100);
                    foreach (var child in header.Children())
                    {
                        child.style.flexGrow = 1;
                        child.style.flexBasis = 0;

                        // Ensure text inside is centered
                        var label = child.Q<Label>();
                        if (label != null) label.style.unityTextAlign = TextAnchor.MiddleCenter;
                    }
                }
            });
        }
    }
}