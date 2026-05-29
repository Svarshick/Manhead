using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Editor.Data;

[Model]
[Prop<Template>("Template")]
[Readonly<Point>("Position")]
public partial class Placement
{
}