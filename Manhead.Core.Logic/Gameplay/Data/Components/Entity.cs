using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

[Model]
[Prop<Color>("Color")]
public partial class Visible : IComponent 
{
}

[Model]
public partial class Player : IComponent 
{
}

[Model]
[Prop<int>("Priority")]
[Prop<float>("Speed")]
public partial class Moving : IComponent 
{
    //public readonly ReactiveProperty<MovementType> MovementType = new();
}