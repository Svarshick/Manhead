using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;
using R3;

namespace Manhead.Core.Logic.Gameplay.Data.Components;

[Model]
[Prop<Color>("Color")]
public partial class Visible : IComponent 
{
    Subject<string> IComponent.Changed => Changed;
}