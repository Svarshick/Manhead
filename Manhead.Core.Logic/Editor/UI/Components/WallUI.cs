using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Manhead.Core.Logic.Editor.UI.Common;
using Manhead.Core.Logic.Gameplay.Data.Components;
using R3;

namespace Manhead.Core.Logic.Editor.UI.Components;

public class WallUI : ContainerRuntime, IDisposable
{
    private readonly IDisposable _subscriptions;

    public WallUI(Wall component)
    {
        var stack = new StackPanel
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };
        this.AddChild(stack);

        var hp = new IntField<Wall.HPProperty>(new Wall.HPProperty(ManheadGame.DefaultSystem, component))
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
        };
        var color = new ColorField<Wall.ColorProperty>(new Wall.ColorProperty(ManheadGame.DefaultSystem, component))
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };

        stack.AddChild(new Field(nameof(Wall.HP), hp));
        stack.AddChild(new Field(nameof(Wall.Color), color));
        _subscriptions = Disposable.Combine(hp, color);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }
}