using Data;
using VContainer;
using VContainer.Unity;

namespace Scenes.GlobalInit
{
    public class Scope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var storage = new SaveStorage(SaveFiles.SavesDir);
            builder.RegisterInstance(storage);
            builder.Register<SceneLoader>(Lifetime.Scoped).Keyed(SceneLoaderType.Internal);
            builder.RegisterEntryPoint<GlobalInit>();
        }
    }
}