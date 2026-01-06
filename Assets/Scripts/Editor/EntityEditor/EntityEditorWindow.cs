using LogicSpace.EditorData;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unit = R3.Unit;

namespace Editor.EntityEditor
{
    [CustomEditor(typeof(EntityData))]
    public class EntityDataInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new HelpBox("Data is managed in the Entity Editor Window.", HelpBoxMessageType.Info));
        
            var openButton = new Button(() => EntityEditorWindow.Open((EntityData)target))
            {
                text = "Open Entity Editor",
                style = { height = 40, marginTop = 10 }
            };
            root.Add(openButton);
            return root;
        }
    } 
    
    public class EntityEditorWindow : EditorWindow
    {
        private enum State
        {
            NullTarget,
            EditTarget,
        }

        private State _currentState;
        private EntityData _currentEntity;
        private EntityEditorViewModel _viewModel;
        private EntityEditorView _view;
    
        [MenuItem("Window/Entity Editor")]
        public static void Open()
        {
            var window = GetWindow<EntityEditorWindow>("Entity Editor");
            var entityData = Selection.activeObject.GetComponent<EntityData>();
            window.SetState(entityData);
        }

        public static void Open(EntityData entityData)
        {
            var window = GetWindow<EntityEditorWindow>("Entity Editor");
            window.SetState(entityData);
        }

        private void OnEnable()
        {
            _currentState = State.NullTarget;
            SetNullUI();
            Selection.selectionChanged += OnSelectionChanged;
        }
        
        private void OnDisable()
        {
            if (_currentState == State.EditTarget)
                DisposeEditor();
            Selection.selectionChanged -= OnSelectionChanged;
        }
        
        private void OnInspectorUpdate()
        {
            //NOTE: gemini says I should check _viewModel.EntitySO.IsValid()
            if (_currentState == State.EditTarget && _currentEntity == null) 
                SetState(null);
            if (_currentState == State.EditTarget)
                _viewModel.Update();
        }
        
        private void OnSelectionChanged()
        {
            GameObject selectedGO = Selection.activeGameObject;
            if (selectedGO != null && 
                _currentState == State.EditTarget &&
                selectedGO.transform.IsChildOf(_currentEntity.transform))
            {
                _viewModel.internalSelectionChanged.OnNext(Unit.Default);
                return;
            }
            var newTargetEntity = selectedGO?.GetComponent<EntityData>();
            SetState(newTargetEntity);
        }
       
        private void SetState(EntityData nextEntity)
        {
            //NOTE: because user can delete _currentEntity, it becomes null, but state is still EditTarget
            if (_currentEntity != null && _currentEntity == nextEntity) 
                return;
            
            if (_currentState == State.EditTarget) 
                DisposeEditor();
            
            _currentEntity = nextEntity;
            _currentState = _currentEntity == null ? State.NullTarget : State.EditTarget;
            if (_currentState == State.EditTarget)
                SetEditorUI();
            if (_currentState == State.NullTarget)
                SetNullUI();
        }

        private void SetNullUI()
        {
            rootVisualElement.Clear();
            var label = new Label($"Please select a GameObject with {nameof(EntityData)}");
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.flexGrow = 1;
            rootVisualElement.Add(label);
        }

        private void SetEditorUI()
        {
            rootVisualElement.Clear();
            _viewModel = new (_currentEntity);
            _view = new (_viewModel, rootVisualElement);
            _view.Initialize();
        }
        
        private void DisposeEditor()
        {
            _view.Dispose();
            _view = null;
            _viewModel.Dispose();
            _viewModel = null;
        }
    }
}