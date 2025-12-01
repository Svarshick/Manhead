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
        
        protected override void Configure(IContainerBuilder builder)
        {
            var safeUIDocument = new SafeUIDocument(uiDocument);
            builder.RegisterInstance(safeUIDocument.Root);
            
            var Map = MapFabric.CreateFromTilemap(tilemap);
            builder.RegisterInstance(Map);
            builder.RegisterEntryPoint<Gameplay>();
            
            builder.Register<MapDebugView>(Lifetime.Singleton);
            
            var debuggerGO = new GameObject("Debugger");
            var MapDebugger =  debuggerGO.AddComponent<MapDebugger>();
            builder.RegisterComponent(MapDebugger);
            
        }
    }
}