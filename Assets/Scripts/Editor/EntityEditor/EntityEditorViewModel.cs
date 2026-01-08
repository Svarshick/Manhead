using System;
using Editor.EntityEditor.Generic;
using LogicSpace.EditorData;
using LogicSpace.EntityAppearance;
using LogicSpace.EntityAppearance.Rules;
using PlayerSpace.UI;
using R3;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.EntityEditor
{
    public class EntityEditorViewModel : ViewModel
    {
        private enum State
        {
            PrefabAsset,
            SceneGO
        }

        private readonly EntityData _entityData;
        private readonly GameObject _prefabAsset;
        private readonly SerializedObject _entitySO;
        private readonly State _state;
        
        public readonly EntityViewModel EntityViewModel;
        public readonly EntitySideViewModel FrontSideViewModel;
        public readonly EntitySideViewModel LeftSideViewModel;
        public readonly EntitySideViewModel RightSideViewModel;
        public readonly EntitySideViewModel BackSideViewModel;

        public readonly Subject<Unit> internalSelectionChanged = new();
        private bool _dataChanged;
        
        private readonly EntityAppearanceBuilder _appearanceBuilder;
        
        public EntityEditorViewModel(EntityData entityData)
        {
            _entityData = entityData ?? throw new ArgumentNullException(nameof(entityData));
            if (PrefabUtility.IsPartOfPrefabAsset(entityData) && entityData.gameObject.transform.parent == null)
            {
                _state = State.PrefabAsset;
                _prefabAsset = _entityData.gameObject;
                var prefabPath = AssetDatabase.GetAssetPath(entityData);
                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                _entityData = root.GetComponent<EntityData>();
            }
            else
            {
                _state = State.SceneGO;
            }

            _entitySO = new SerializedObject(_entityData);

            Action dataChangedAction = () => _dataChanged = true;
            EntityViewModel = new EntityViewModel(_entitySO, dataChangedAction);
            var leftSideProperty = _entitySO.FindProperty("LeftSide");
            var rightSideProperty = _entitySO.FindProperty("RightSide");
            var frontSideProperty = _entitySO.FindProperty("FrontSide");
            var backSideProperty = _entitySO.FindProperty("BackSide");
            LeftSideViewModel = new EntitySideViewModel(_entitySO, leftSideProperty, dataChangedAction);
            RightSideViewModel = new EntitySideViewModel(_entitySO, rightSideProperty, dataChangedAction);
            FrontSideViewModel = new EntitySideViewModel(_entitySO, frontSideProperty, dataChangedAction);
            BackSideViewModel = new EntitySideViewModel(_entitySO, backSideProperty, dataChangedAction);

            var arrowRule = new ArrowEntityVisualizationRule();
            var baseRule = new BaseEntityVisualizationRule();
            _appearanceBuilder = new EntityAppearanceBuilder(new IEntityVisualizationRule[] { arrowRule, baseRule });
        }

        public void Update()
        {
            if (!_dataChanged)
                return;
            _dataChanged = false;

            ResetAppearance();
            if (_state == State.PrefabAsset)
                PrefabUtility.SaveAsPrefabAsset(_entityData.gameObject, AssetDatabase.GetAssetPath(_prefabAsset));

            EntityViewModel.Update();
            LeftSideViewModel.Update();
            RightSideViewModel.Update();
            FrontSideViewModel.Update();
            BackSideViewModel.Update();
        }

        private void ResetAppearance()
        {
            var t = _entityData.transform;
            //NOTE: side effect
            while (t.childCount > 0) Object.DestroyImmediate(t.GetChild(0).gameObject);

            var appearanceGO = _appearanceBuilder.BuildAppearance(_entityData);

            if (appearanceGO != null) appearanceGO.transform.SetParent(t, false);
        }
    }
}