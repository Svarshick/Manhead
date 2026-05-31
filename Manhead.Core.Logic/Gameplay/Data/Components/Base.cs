using R3;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

public interface IComponent : IDisposable
{
    public Subject<string> Changed { get; }
}

public interface ISideComponent : IDisposable
{
    
}