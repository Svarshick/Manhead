using System.Reflection;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

public static class ComponentRegistry
{
    public static readonly Type[] SideComponents;
    public static readonly Type[] EntityComponents;

    static ComponentRegistry()
    {
        var assembly = typeof(ComponentRegistry).Assembly;

        var componentsWithAttributes = assembly.GetTypes()
            .Where(t => typeof(IComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttribute<ComponentAttribute>()
            })
            .Where(x => x.Attribute != null)
            .ToList();

        SideComponents = componentsWithAttributes
            .Where(x => x.Attribute.Kind == Kind.Side)
            .Select(x => x.Type)
            .ToArray();

        EntityComponents = componentsWithAttributes
            .Where(x => x.Attribute.Kind == Kind.Entity)
            .Select(x => x.Type)
            .ToArray();
    }
}