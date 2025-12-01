using PlayerSpace.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace LogicSpace
{
    public class MapDebugView : View 
    {
        private FieldDebugInfoView _fieldDebugInfoView;
        
        public MapDebugView(VisualElement root) : base(root) { }

        protected override void SetVisualElements()
        {
            _fieldDebugInfoView = CreateFieldDebugInfoView(Root);
        }

        private FieldDebugInfoView CreateFieldDebugInfoView(VisualElement root)
        {
            var debugInfoAsset = Resources.Load<VisualTreeAsset>("debug-info");
            var fieldDebugRoot = debugInfoAsset.Instantiate();
            root.Add(fieldDebugRoot);
            var fieldDebug = new FieldDebugInfoView(fieldDebugRoot);
            fieldDebug.Initialize();
            return fieldDebug;
        }

        public void ShowFieldDebug(Vector2 worldPosition)
        {
            var position = RuntimePanelUtils.CameraTransformWorldToPanel(Root.panel, worldPosition, Camera.main);
            _fieldDebugInfoView.Root.style.left = position.x;
            _fieldDebugInfoView.Root.style.top = position.y;
            _fieldDebugInfoView.Show();
        }

        public void HideFieldDebug() => _fieldDebugInfoView.Hide();

        public void SetInfo(string info) => _fieldDebugInfoView.SetInfo(info);
    }
    
    public class FieldDebugInfoView : View
    {
        private Label _label;
        
        public FieldDebugInfoView(VisualElement root, bool hideOnAwake = true) : base(root, hideOnAwake)
        {
        }

        protected override void SetVisualElements()
        {
            _label = Root.Q<Label>("label");
        }

        protected override void BindViewData()
        {
            _label.text = "None";
        }

        public void SetInfo(string info) => _label.text = info;
    }
}