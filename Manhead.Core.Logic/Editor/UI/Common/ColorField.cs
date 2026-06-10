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
    private sealed class TextSlider : ContainerRuntime 
    {
        public readonly Slider Slider;
        public readonly Label Label;

        public TextSlider()
        {
            var grid = new Grid
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                Width = 0,
                Height = 0,
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                }
            };
            this.AddChild(grid);
                
            Label = new Label();
            Slider = new Slider
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                Width = 0,
            };
            grid.AddChild(Label, 0, 0);
            grid.AddChild(Slider, 0, 1);
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