using System.Linq;
using PlayerSpace.UI;
using R3;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.EntityEditor.Generic
{
    public class ComponentsView<TComponent> : View<ComponentsViewModel<TComponent>>
    {
        private Button _addComponentButton;
        private ScrollView _container;

        public ComponentsView(ComponentsViewModel<TComponent> model, VisualElement root) : base(model, root)
        {
        }

        protected override void SetVisualElements()
        {
            _container = new ScrollView(ScrollViewMode.Vertical);
            Root.Add(_container);

            _addComponentButton = new Button(OpenAddComponentSearch)
            {
                text = "Add Component",
                style =
                {
                    height = 30,
                    marginTop = 10
                }
            };
            Root.Add(_addComponentButton);
        }

        protected override void BindViewData()
        {
            ViewModel.componentsListChanged
                .Subscribe(_ => RebuildList())
                .AddTo(Disposables);

            RebuildList();
        }

        private void RebuildList()
        {
            _container.Clear();
            var listProp = ViewModel.ListSP;

            for (var i = 0; i < listProp.arraySize; i++)
            {
                var elementProp = listProp.GetArrayElementAtIndex(i);

                // --- A. Container Box ---
                var box = new VisualElement();
                box.style.borderBottomWidth = 1;
                box.style.borderBottomColor = Color.black;
                box.style.marginBottom = 5;
                box.style.backgroundColor = new StyleColor(new Color(0.22f, 0.22f, 0.22f));

                // --- B. Header (Name + Remove Button) ---
                var header = new VisualElement();
                header.style.flexDirection = FlexDirection.Row;
                header.style.justifyContent = Justify.SpaceBetween;
                header.style.paddingBottom = 5;

                var typeName = elementProp.managedReferenceFullTypename.Split(' ').Last();
                var label = new Label(ObjectNames.NicifyVariableName(typeName));
                label.style.unityFontStyleAndWeight = FontStyle.Bold;

                var index = i; // Capture index for closure
                var removeBtn = new Button(() => ViewModel.RemoveComponent(index)) { text = "X" };

                header.Add(label);
                header.Add(removeBtn);
                box.Add(header);

                // --- C. Properties (Flattened) ---
                // We do not use new PropertyField(elementProp) because it creates an "Element X" foldout.
                // Instead, we iterate children manually.

                var endProp = elementProp.GetEndProperty();
                var childProp = elementProp.Copy();

                // "NextVisible(true)" enters the children of the class
                if (childProp.NextVisible(true))
                    do
                    {
                        // Stop if we exit the component's data
                        if (SerializedProperty.EqualContents(childProp, endProp)) break;

                        // Draw the field (Float, Int, Vector3, etc)
                        var field = new PropertyField(childProp);
                        field.style.paddingLeft = 10;
                        field.Bind(ViewModel.TargetSO);
                        if (ViewModel.dataChangedAction != null)
                            field.RegisterValueChangeCallback(_ => ViewModel.dataChangedAction.Invoke());

                        box.Add(field);
                    }
                    // "NextVisible(false)" moves to the next sibling field (e.g. Health -> MaxHealth)
                    while (childProp.NextVisible(false));

                _container.Add(box);
            }
        }

        private void OpenAddComponentSearch()
        {
            var searchProvider = ScriptableObject.CreateInstance<TypeSearchProvider>();
            searchProvider.Init(typeof(TComponent), type => { ViewModel.AddComponent(type); }, "Components");

            var worldBound = _addComponentButton.worldBound;
            var searchPosition = GUIUtility.GUIToScreenPoint(worldBound.position);
            searchPosition.y += worldBound.height;

            SearchWindow.Open(new SearchWindowContext(searchPosition, worldBound.width), searchProvider);
        }
    }
}