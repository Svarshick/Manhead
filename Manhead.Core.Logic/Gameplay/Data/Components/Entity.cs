using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;
using R3;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

[Component(Kind.Entity)]
[Model]
[Prop<Color>("Color")]
public partial class Visible : IComponent 
{
    Subject<string> IComponent.Changed => Changed;
    public IComponent Clone() => new Visible { Color = Color };
}

[Component(Kind.Entity)]
[Model]
public partial class Player : IComponent
{
    Subject<string> IComponent.Changed => Changed;
    public IComponent Clone() => new Player();
}

[Component(Kind.Entity)]
[Model]
[Prop<float>("Speed")]
public partial class Moving : IComponent
{
    Subject<string> IComponent.Changed => Changed;
    public IComponent Clone() => new Moving() { Speed = Speed };
}