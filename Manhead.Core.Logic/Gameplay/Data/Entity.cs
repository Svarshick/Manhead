using Manhead.Core.Logic.Gameplay.Data.Components;
using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Gameplay.Data;

[Model]
[Prop<EntitySide>("FrontSide")]
[Prop<EntitySide>("BackSide")]
[Prop<EntitySide>("LeftSide")]
[Prop<EntitySide>("RightSide")]
[List<IComponent>("Components")]
[Prop<Point>("Position")]
public partial class Entity
{
}

[Model]
[List<ISideComponent>("Components")]
public partial class EntitySide
{
}