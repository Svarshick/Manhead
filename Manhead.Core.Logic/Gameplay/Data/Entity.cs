using Manhead.Core.Logic.Gameplay.Data.Components;
using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;
using ObservableCollections;

namespace Manhead.Core.Logic.Gameplay.Data;

public interface IComponentHolder
{
    IReadOnlyObservableList<IComponent> Components { get; }
    bool HasComponent(Type type);
    T? GetComponent<T>() where T : class, IComponent;
    void AddComponent<T>(T component) where T : class, IComponent;
    void RemoveComponent(Type type);
    void RemoveComponent<T>() where T : class, IComponent;
    
}

[Model]
[Prop<Point>("Position")]
public partial class Entity : IComponentHolder
{
    public readonly EntitySide LeftSide = new();
    public readonly EntitySide RightSide = new();
    public readonly EntitySide FrontSide = new();
    public readonly EntitySide BackSide = new();

    public IReadOnlyObservableList<IComponent> Components => _components;
    private readonly ObservableList<IComponent> _components = new();
    
    public bool HasComponent(Type type)
    {
        foreach (var component in _components)
        {
            if (component.GetType().IsAssignableFrom(type))
                return true;
        }

        return false;
    }
    
    public T? GetComponent<T>() where T : class, IComponent
    {
        foreach (var component in _components)
        {
            if (component is T match)
                return match;
        }

        return null;
    }
    
    public void AddComponent<T>(T component) where T : class, IComponent
    {
        var existingComponent = GetComponent<T>();
        if (existingComponent != null)
            throw new ArgumentException($"The component {component.GetType().Name} is already attached");
        
        _components.Add(component);
        Changed.OnNext(nameof(Components));
    }

    public void RemoveComponent(Type type)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].GetType() == type)
            {
                _components[i].Dispose();
                _components.RemoveAt(i);
                Changed.OnNext(nameof(Components));
                return;
            }
        }

        throw new ArgumentException($"The component {type.Name} isn't attached");
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
public partial class EntitySide : IComponentHolder
{
    public IReadOnlyObservableList<IComponent> Components => _components;
    private readonly ObservableList<IComponent> _components = new();
       
     public bool HasComponent(Type type)
    {
        foreach (var component in _components)
        {
            if (component.GetType().IsAssignableFrom(type))
                return true;
        }

        return false;
    }
     
    public T? GetComponent<T>() where T : class, IComponent
    {
        foreach (var component in _components)
        {
            if (component is T match)
                return match;
        }

        return null;
    }
    
    public void AddComponent<T>(T component) where T : class, IComponent
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
    
    public void RemoveComponent(Type type)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].GetType() == type)
            {
                _components[i].Dispose();
                _components.RemoveAt(i);
                Changed.OnNext(nameof(Components));
                return;
            }
        }

        throw new ArgumentException($"The component {type.Name} isn't attached");
    }
}