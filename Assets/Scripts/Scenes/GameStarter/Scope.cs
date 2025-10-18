using VContainer;
using VContainer.Unity;

namespace Scenes.GameStarter
{
    public class Scope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameStarter>();
        }
    }
}