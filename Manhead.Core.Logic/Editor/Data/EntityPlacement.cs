using Manhead.Core.Logic.Gameplay.Data;
using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Editor.Data;

[Model]
[Prop<Entity>("Template")]
[Readonly<Point>("Position")]
public partial class EntityPlacement
{
}