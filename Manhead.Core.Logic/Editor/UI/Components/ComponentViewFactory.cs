using Gum.Wireframe;
using Manhead.Core.Logic.Gameplay.Data.Components;

namespace Manhead.Core.Logic.Editor.UI.Components;

public static class ComponentViewFactory
{
    public static GraphicalUiElement Create(IComponent component, out IDisposable disposable)
    {
        switch (component)
        {
            case Visible visible:
            {
                var view = new VisibleView(visible);
                disposable = view;
                return view;
            }
            case Wall visible:
            {
                var view = new WallView(visible);
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