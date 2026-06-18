using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Manhead.Core.Logic.Editor.UI.Common;
using Manhead.Core.Logic.Gameplay.Data.Components;
using R3;

namespace Manhead.Core.Logic.Editor.UI.Components;

public class VisibleUI : ContainerRuntime, IDisposable 
{
    private readonly IDisposable _subscriptions;
    
    public VisibleUI(Visible component)
    {
        var stack = new StackPanel
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };
        this.AddChild(stack);

        var color = new ColorField<Visible.ColorProperty>(new Visible.ColorProperty(Game.DefaultSystem, component))
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };
        
        stack.AddChild(new Field(nameof(Manhead.Core.Logic.Gameplay.Data.Components.Visible.Color), color));
        _subscriptions = Disposable.Combine(color);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }
}