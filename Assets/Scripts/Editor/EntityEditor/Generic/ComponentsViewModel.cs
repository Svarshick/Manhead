using System;
using PlayerSpace.UI;
using R3;
using UnityEditor;

namespace Editor.EntityEditor.Generic
{
    public class ComponentsViewModel<TComponent> : ViewModel
    {
        public readonly SerializedProperty ListSP;
        public readonly SerializedObject TargetSO;
        public readonly Action dataChangedAction;

        public readonly Subject<Unit> componentsListChanged;
        private long _lastStructureHash;

        public ComponentsViewModel(
            SerializedObject targetSO,
            SerializedProperty listSP,
            Action dataChangedAction = null)
        {
            TargetSO = targetSO ?? throw new ArgumentNullException(nameof(targetSO));
            ListSP = listSP ?? throw new ArgumentNullException(nameof(listSP));
            //TODO: refactor naming
            this.dataChangedAction = dataChangedAction;
            componentsListChanged = new Subject<Unit>();

            _lastStructureHash = ComputeStructureHash();
        }

        public void AddComponent(Type componentType)
        {
            //TODO: should I add null check?
            TargetSO.Update();
            ListSP.arraySize++;
            var newElement = ListSP.GetArrayElementAtIndex(ListSP.arraySize - 1);
            newElement.managedReferenceValue = (TComponent)Activator.CreateInstance(componentType);

            TargetSO.ApplyModifiedProperties();
            CheckHashAndNotify();
        }

        public void RemoveComponent(int index)
        {
            TargetSO.Update();
            ListSP.DeleteArrayElementAtIndex(index);
            TargetSO.ApplyModifiedProperties();
            CheckHashAndNotify();
        }

        public void Update(bool forceUpdate = false)
        {
            //NOTE: notifies only component list structure changed (added or removed component), doesn't care about component property changes
            if (forceUpdate || TargetSO.UpdateIfRequiredOrScript())
                CheckHashAndNotify();
        }

        private void CheckHashAndNotify()
        {
            var currentHash = ComputeStructureHash();
            if (currentHash != _lastStructureHash)
            {
                _lastStructureHash = currentHash;
                componentsListChanged.OnNext(Unit.Default);
                dataChangedAction.Invoke();
            }
        }

        //NOTE: Gemini generated
        private long ComputeStructureHash()
        {
            if (ListSP == null) return 0;

            unchecked // Allow integer overflow (standard for hashing)
            {
                long hash = 17;
                hash = hash * 23 + ListSP.arraySize;

                for (var i = 0; i < ListSP.arraySize; i++)
                {
                    var element = ListSP.GetArrayElementAtIndex(i);
                    // managedReferenceId is unique to the object instance. 
                    // It persists through value changes and reordering.
                    hash = hash * 23 + element.managedReferenceId;
                }

                return hash;
            }
        }

        public override void Dispose()
        {
            componentsListChanged.Dispose();
        }
    }
}