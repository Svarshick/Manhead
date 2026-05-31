using Gum.Wireframe;
using Manhead.Core.Logic.Gameplay.Data.Components;

namespace Manhead.Core.Logic.Editor.UI.Components;

public static class ComponentViewFabric
{
    public static GraphicalUiElement Create(IComponent component, out IDisposable subscriptions)
    {
        switch (component)
        {
            case Visible visible:
                return visible.CreateView(out subscriptions);
            default:
                throw new NotImplementedException("Haha, there is no UI for other");
        }
    }
}