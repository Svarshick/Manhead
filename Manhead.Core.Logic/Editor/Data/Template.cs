using Manhead.Core.Logic.Gameplay.Data;
using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Editor.Data;

[Model]
[Prop<string>("Name")]
[Prop<Entity>("Entity")]
public partial class Template
{
}