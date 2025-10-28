using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;
using VContainer.Unity;

namespace LogicSpace
{
    public class Scope : LifetimeScope
    {
        [SerializeField] private Tilemap tilemap;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(tilemap);
            builder.RegisterEntryPoint<Gameplay>();
        }
    }
}