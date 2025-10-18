using PlayerSpace.UI.MainMenu;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace Scenes.MainMenu
{
    public class Scope : LifetimeScope
    {
        [FormerlySerializedAs("MainMenuUI")] [SerializeField] private UIDocument mainMenuUI;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SceneLoader>(Lifetime.Scoped)
                .Keyed(SceneLoaderType.Internal);
            builder.Register<SceneLoader>(
                    _ => Parent.Container.Resolve<SceneLoader>(SceneLoaderType.Internal), 
                    Lifetime.Scoped)
                .Keyed(SceneLoaderType.External);
            
            builder.Register<MainMenuViewModel>(Lifetime.Scoped);
            builder.RegisterInstance(mainMenuUI.rootVisualElement);
            builder.RegisterEntryPoint<MainMenuView>();
        }
    }
}