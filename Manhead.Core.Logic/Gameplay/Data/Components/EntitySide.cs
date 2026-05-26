using Manhead.Core.Logic.WorldSpace;
using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

[Model]
[Prop<Direction>("RotationDirection")]
public partial class Crossroad : ISideComponent
{
}

[Model]
public partial class Stop : ISideComponent
{
}