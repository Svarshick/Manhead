using Gum.GueDeriving;
using Gum.Wireframe;
using Manhead.Core.Logic.Gameplay.Data.Components;

namespace Manhead.Core.Logic.Editor.UI.Components;

public static class ComponentUIFactory
{
    public static GraphicalUiElement Create(IComponent component, out IDisposable? disposable)
    {
        switch (component)
        {
            case Visible visible:
            {
                var view = new VisibleUI(visible);
                disposable = view;
                return view;
            }
            case Wall wall:
            {
                var view = new WallUI(wall);
                disposable = view;
                return view;
            }
            case Player player:
            {
                disposable = null;
                return new ContainerRuntime();
            }
            case Moving moving:
            {
                var view = new MovingUI(moving);
                disposable = view;
                return view;
            }
            default:
            {
                throw new NotImplementedException($"Haha, there is no UI for {component.GetType()}");
            }
        }
    }
}