using LogicSpace.Fields;
using PlayerSpace.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace LogicSpace
{
    public class Scope : LifetimeScope
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] public bool randomization;
        [SerializeField] public int crossroadsAmount;
        [SerializeField] public int stepDelayTime;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<VisualElement>(
                _ => new SafeUIDocument(uiDocument).Root,
                Lifetime.Singleton);
            builder.Register<Map>(_ =>
                {
                    if (randomization)
                        return MapFactory.CreateFromTilemapWithCrossroadsGeneration(tilemap, crossroadsAmount);
                    return MapFactory.CreateFromTilemap(tilemap);
                }, 
                Lifetime.Singleton);
            builder.RegisterInstance(stepDelayTime);
            builder.RegisterEntryPoint<Gameplay>();
            RegisterDebug(builder);
        }

        private void RegisterDebug(IContainerBuilder builder)
        {
            builder.Register<MapDebugView>(Lifetime.Singleton);

            var debuggerGO = new GameObject("Debugger");
            var MapDebugger = debuggerGO.AddComponent<MapDebugger>();
            builder.RegisterComponent(MapDebugger); 
        }
    }
}