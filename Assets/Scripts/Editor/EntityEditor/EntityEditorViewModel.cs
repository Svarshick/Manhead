using System;
using Editor.EntityEditor.Generic;
using LogicSpace.EditorData;
using LogicSpace.EntityAppearance;
using LogicSpace.EntityAppearance.Rules;
using PlayerSpace.UI;
using R3;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Editor.EntityEditor
{
    public class EntityEditorViewModel : ViewModel
    {
        private readonly EntityAppearanceBuilder _appearanceBuilder;
        private readonly string _prefabPath;
        public readonly EntitySideViewModel BackSideViewModel;
        public readonly EntityData EntityData;
        public readonly SerializedObject EntitySO;

        public readonly EntityViewModel EntityViewModel;
        public readonly EntitySideViewModel FrontSideViewModel;
        public readonly Subject<Unit> internalSelectionChanged = new();
        public readonly EntitySideViewModel LeftSideViewModel;
        public readonly EntitySideViewModel RightSideViewModel;

        private bool _dataChanged;

        public EntityEditorViewModel(EntityData entityData)
        {
            EntityData = entityData ?? throw new ArgumentNullException(nameof(entityData));
            if (PrefabUtility.IsPartOfPrefabAsset(entityData) && entityData.gameObject.transform.parent == null)
            {
                _prefabPath = AssetDatabase.GetAssetPath(entityData);
                var rootGO = PrefabUtility.LoadPrefabContents(_prefabPath);
                EntityData = rootGO.GetComponent<EntityData>();
            }

            EntitySO = new SerializedObject(EntityData);

            Action dataChangedAction = () => _dataChanged = true;
            EntityViewModel = new EntityViewModel(EntitySO, dataChangedAction);
            var leftSideProperty = EntitySO.FindProperty("LeftSide");
            var rightSideProperty = EntitySO.FindProperty("RightSide");
            var frontSideProperty = EntitySO.FindProperty("FrontSide");
            var backSideProperty = EntitySO.FindProperty("BackSide");
            LeftSideViewModel = new EntitySideViewModel(EntitySO, leftSideProperty, dataChangedAction);
            RightSideViewModel = new EntitySideViewModel(EntitySO, rightSideProperty, dataChangedAction);
            FrontSideViewModel = new EntitySideViewModel(EntitySO, frontSideProperty, dataChangedAction);
            BackSideViewModel = new EntitySideViewModel(EntitySO, backSideProperty, dataChangedAction);

            var arrowRule = new ArrowEntityVisualizationRule();
            var baseRule = new BaseEntityVisualizationRule();
            _appearanceBuilder = new EntityAppearanceBuilder(new IEntityVisualizationRule[] { arrowRule, baseRule });
        }

        public void Update()
        {
            if (!EntitySO.UpdateIfRequiredOrScript())
                return;

            if (!_dataChanged)
                return;
            _dataChanged = false;

            ResetAppearance();
            if (_prefabPath != null) PrefabUtility.SaveAsPrefabAsset(EntityData.gameObject, _prefabPath);

            EntityViewModel.Update();
            LeftSideViewModel.Update();
            RightSideViewModel.Update();
            FrontSideViewModel.Update();
            BackSideViewModel.Update();
        }

        private void ResetAppearance()
        {
            var t = EntityData.transform;
            //NOTE: side effect
            while (t.childCount > 0) Object.DestroyImmediate(t.GetChild(0).gameObject);

            var appearanceGO = _appearanceBuilder.BuildAppearance(EntityData);

            if (appearanceGO != null) appearanceGO.transform.SetParent(t, false);
        }
    }
}