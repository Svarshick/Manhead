using R3;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

[Flags]
public enum Kind
{
    Entity = 1,
    Side   = 2,
    All = Entity | Side
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ComponentAttribute(Kind kind) : Attribute
{
    public Kind Kind { get; } = kind;
}


public interface IComponent : IDisposable
{
    public Subject<string> Changed { get; }
}