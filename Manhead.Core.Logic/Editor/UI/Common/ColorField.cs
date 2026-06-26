using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Microsoft.Xna.Framework;
using ModelMediator.Abstractions;
using R3;

namespace Manhead.Core.Logic.Editor.UI.Common;

public class ColorField<TProperty> : ContainerRuntime, IDisposable
    where TProperty : struct, IProperty<Color>
{
    private sealed class TextSlider : StackPanel 
    {
        public readonly Slider Slider;
        public readonly Label Label;

        public TextSlider()
        {
            WidthUnits = DimensionUnitType.RelativeToParent;
            HeightUnits = DimensionUnitType.RelativeToChildren;
            Width = 0;
            Height = 0;
            Orientation = Orientation.Horizontal;

            Label = new Label
            {
                WidthUnits = DimensionUnitType.Absolute,
                Width = 50,
            };
            Slider = new Slider
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                Width = -Label.Width,
            };
            AddChild(Label);
            AddChild(Slider);
        }
    }
    
    private readonly IDisposable _subscriptions;
    
    public ColorField(TProperty property)
    {
        var stack = new StackPanel
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };
        this.AddChild(stack);

        var rSlider = new TextSlider
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
            Slider =
            {
                Minimum = 0,
                Maximum = 255,
                Value = property.Value.R
            },
            Label =
            {
                Text = $"r: {property.Value.R}"
            }
        };
        stack.AddChild(rSlider);

        var gSlider = new TextSlider
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
            Slider =
            {
                Minimum = 0,
                Maximum = 255,
                Value = property.Value.G
            },
            Label =
            {
                Text = $"g: {property.Value.G}"
            }
        };
        stack.AddChild(gSlider);
        
        var bSlider = new TextSlider
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
            Slider =
            {
                Minimum = 0,
                Maximum = 255,
                Value = property.Value.B
            },
            Label =
            {
                Text = $"b: {property.Value.B}"
            }
        };
        stack.AddChild(bSlider);

        var aSlider = new TextSlider
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
            Slider =
            {
                Minimum = 0,
                Maximum = 255,
                Value = property.Value.A
            },
            Label =
            {
                Text = $"a: {property.Value.A}"
            }
        };
        stack.AddChild(aSlider);

        var preview = new RectangleRuntime
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.Absolute,
            Width = 0,
            Height = 30,
            FillColor = property.Value,
            IsFilled = true
        };
        stack.AddChild(preview);

        rSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            property.TrySet(property.Value with { R = (byte)rSlider.Slider.Value });
        };
        gSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            property.TrySet(property.Value with { G = (byte)gSlider.Slider.Value });
        };
        bSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            property.TrySet(property.Value with { B = (byte)bSlider.Slider.Value });
        };
        aSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            property.TrySet(property.Value with { A = (byte)aSlider.Slider.Value });
        };
        
        var rSubscription= property.Changed
            .Subscribe(rSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.R;
                slider.Label.Text = $"r: {next.R}";
            });
        var gSubscription= property.Changed
            .Subscribe(gSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.G;
                slider.Label.Text = $"g: {next.G}";
            });
        var bSubscription= property.Changed
            .Subscribe(bSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.B;
                slider.Label.Text = $"b: {next.B}";
            });
        var aSubscription= property.Changed
            .Subscribe(aSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.A;
                slider.Label.Text = $"a: {next.A}";
            });
        var previewSubscription = property.Changed
            .Subscribe(preview, static (next, preview) =>
            {
                preview.FillColor = next;
            });

        _subscriptions = Disposable.Combine(
            rSubscription,
            gSubscription,
            bSubscription,
            aSubscription,
            previewSubscription);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }
}