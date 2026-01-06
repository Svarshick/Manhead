using LogicSpace.EditorData;
using LogicSpace.GameField;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LogicSpace
{
    public class Scope : LifetimeScope
    {
        [SerializeField] private FieldData fieldData;
        [SerializeField] private int randomCrossroads;
        [SerializeField] private int stepDelayTime;

        protected override void Configure(IContainerBuilder builder)
        {
            /*builder.Register<VisualElement>(
                _ => new SafeUIDocument(uiDocument).Root,
                Lifetime.Singleton);*/
            builder.Register(_ => FieldFactory.CreateFromDataAndSpawnCrossroads(fieldData, randomCrossroads),
                Lifetime.Singleton);
            builder.RegisterInstance(stepDelayTime);
            builder.RegisterEntryPoint<Gameplay>();
        }
    }
}