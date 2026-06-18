using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Manhead.Core.Logic.Editor.UI.Common;
using Manhead.Core.Logic.Gameplay.Data.Components;
using R3;

namespace Manhead.Core.Logic.Editor.UI.Components;

public class MovingUI : ContainerRuntime, IDisposable
{
    private readonly IDisposable _subscriptions;

    public MovingUI(Moving component)
    {
        var stack = new StackPanel
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };
        this.AddChild(stack);

        var speed = new FloatField<Moving.SpeedProperty>(new Moving.SpeedProperty(Game.DefaultSystem, component))
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
        };

        stack.AddChild(new Field(nameof(Moving.Speed), speed));
        _subscriptions = Disposable.Combine(speed);
    }
    
    public void Dispose()
    {
        _subscriptions.Dispose();
    }
}