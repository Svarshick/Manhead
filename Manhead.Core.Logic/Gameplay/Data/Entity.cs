using Manhead.Core.Logic.Gameplay.Data.Components;
using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Gameplay.Data;

[Model]
[Prop<Point>("Position")]
public partial class Entity
{
    public readonly EntitySide FrontSide = new();
    public readonly EntitySide BottomSide = new();
    public readonly EntitySide LeftSide = new();
    public readonly EntitySide RightSide = new();
    
    private readonly List<IComponent> _components = new();
    
    public IEnumerable<IComponent> Components => _components;
    
    public T? GetComponent<T>() where T : class, IComponent
    {
        foreach (var component in _components)
        {
            if (component is T match)
                return match;
        }

        return null;
    }
    
    internal void AddComponent<T>(T component) where T : class, IComponent
    {
        var existingComponent = GetComponent<T>();
        if (existingComponent != null)
            throw new ArgumentException($"The component {component.GetType().Name} is already attached");
        
        _components.Add(component);
        Changed.OnNext(nameof(Components));
    }
    
    public void RemoveComponent<T>() where T : class, IComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                _components[i].Dispose();
                _components.RemoveAt(i);
                Changed.OnNext(nameof(Components));
                return;
            }
        }

        throw new ArgumentException($"The component {typeof(T).Name} isn't attached");
    }
}

[Model]
public partial class EntitySide
{
    private readonly List<ISideComponent> _components = new();
    
    public IEnumerable<ISideComponent> Components => _components;
    
    public T? GetComponent<T>() where T : class, ISideComponent
    {
        foreach (var component in _components)
        {
            if (component is T match)
                return match;
        }

        return null;
    }
    
    internal void AddComponent<T>(T component) where T : class, ISideComponent
    {
        var existingComponent = GetComponent<T>();
        if (existingComponent != null)
            throw new ArgumentException($"The component {component.GetType().Name} is already attached");
        
        _components.Add(component);
        Changed.OnNext(nameof(Components));
    }
    
    public void RemoveComponent<T>() where T : class, ISideComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                _components[i].Dispose();
                _components.RemoveAt(i);
                Changed.OnNext(nameof(Components));
                return;
            }
        }

        throw new ArgumentException($"The component {typeof(T).Name} isn't attached");
    }
}