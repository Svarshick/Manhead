using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;
using R3;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

[Component(Kind.Side)]
[Model]
[Prop<Direction>("Direction")]
public partial class Crossroad : IComponent
{
    Subject<string> IComponent.Changed => Changed;
    public IComponent Clone() => new Crossroad { Direction = Direction };
}

[Component(Kind.Side)]
[Model]
[Prop<int>("HP")]
[Prop<Color>("Color")]
public partial class Wall : IComponent
{
    Subject<string> IComponent.Changed => Changed;
    public IComponent Clone() => new Wall { HP = HP, Color = Color };
}