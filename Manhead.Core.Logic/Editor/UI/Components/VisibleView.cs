using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Manhead.Core.Logic.Gameplay.Data.Components;
using R3;
using MonoGameGum;

namespace Manhead.Core.Logic.Editor.UI.Components;

public static class VisibleView
{
    public static GraphicalUiElement CreateView(this Visible component, out IDisposable subscriptions)
    {
        var stack = new StackPanel
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };

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
                Value = component.Color.R
            },
            Label =
            {
                Text = $"r: {component.Color.R}"
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
                Value = component.Color.G
            },
            Label =
            {
                Text = $"g: {component.Color.G}"
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
                Value = component.Color.B
            },
            Label =
            {
                Text = $"b: {component.Color.B}"
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
                Value = component.Color.A
            },
            Label =
            {
                Text = $"a: {component.Color.A}"
            }
        };
        stack.AddChild(aSlider);

        var preview = new RectangleRuntime
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.Absolute,
            Width = 0,
            Height = 30,
            FillColor = component.Color,
        };
        stack.AddChild(preview);

        rSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            component.Color = component.Color with { R = (byte)rSlider.Slider.Value };
        };
        gSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            component.Color = component.Color with { G = (byte)gSlider.Slider.Value };
        };
        bSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            component.Color = component.Color with { B = (byte)bSlider.Slider.Value };
        };
        aSlider.Slider.ValueChangedByUi += (_, _) =>
        {
            component.Color = component.Color with { A = (byte)aSlider.Slider.Value };
        };
            
        var rSubscription= component.Changed
            .Where(prop => prop == nameof(Visible.Color))
            .Select(component, static (_, component) => component.Color)
            .Subscribe(rSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.R;
                slider.Label.Text = $"r: {next.R}";
            });
        var gSubscription= component.Changed
            .Where(prop => prop == nameof(Visible.Color))
            .Select(component, static (_, component) => component.Color)
            .Subscribe(gSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.G;
                slider.Label.Text = $"g: {next.G}";
            });
        var bSubscription= component.Changed
            .Where(prop => prop == nameof(Visible.Color))
            .Select(component, static (_, component) => component.Color)
            .Subscribe(bSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.B;
                slider.Label.Text = $"b: {next.B}";
            });
        var aSubscription= component.Changed
            .Where(prop => prop == nameof(Visible.Color))
            .Select(component, static (_, component) => component.Color)
            .Subscribe(aSlider, static (next, slider) =>
            {
                slider.Slider.Value = next.A;
                slider.Label.Text = $"a: {next.A}";
            });
        var previewSubscription = component.Changed
            .Where(prop => prop == nameof(Visible.Color))
            .Select(component, static (_, component) => component.Color)
            .Subscribe(preview, static (next, preview) =>
            {
                preview.FillColor = next;
            });

        subscriptions = Disposable.Combine(
            rSubscription,
            gSubscription,
            bSubscription,
            aSubscription,
            previewSubscription);
        return stack.Visual;
    }

    public sealed class TextSlider : ContainerRuntime 
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
                    new ColumnDefinition(new GridLength(1, GridUnitType.Absolute)),
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
}