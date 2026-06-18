using Manhead.Core.Logic.Gameplay.Data;

namespace Manhead.Core.Logic.Editor.Data;

public class LevelBlueprint
{
    public readonly Field<Placement> Field;

    public LevelBlueprint(Field<Placement> field)
    {
        Field = field;
    }
}