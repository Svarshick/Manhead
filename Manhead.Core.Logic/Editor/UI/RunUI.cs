using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using RenderingLibrary.Graphics;

namespace Manhead.Core.Logic.Editor.UI;

public class RunUI : ContainerRuntime 
{
    public RunUI(EventBus eventBus)
    {
        WidthUnits = DimensionUnitType.RelativeToParent;
        HeightUnits = DimensionUnitType.RelativeToParent;
        Width = 0;
        Height = 0;
        this.AddChild(CreateTopPanel());
        return;
        
        Grid CreateTopPanel()
        {
            var panel = new Grid
            {
                XOrigin = HorizontalAlignment.Center,
                YOrigin = VerticalAlignment.Top,
                XUnits = GeneralUnitType.Percentage,
                YUnits = GeneralUnitType.Percentage,
                X = 50,
                Y = 0,
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.Absolute,
                Width = 40,
                Height = 50,
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                }
            };

            var exitButton = new Button
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                Height = 100,
                Text = $"Exit",
            };
            exitButton.Click += (_, _) => eventBus.RaiseExitLevel();
            panel.AddChild(exitButton, 0, 0);

            for (int i = 1; i < 4; i++)
            {
                var button = new Button
                {
                    WidthUnits = DimensionUnitType.PercentageOfParent,
                    HeightUnits = DimensionUnitType.PercentageOfParent,
                    Width = 100,
                    Height = 100,
                    Text = $"Tool {i}"
                };
                panel.AddChild(button, 0, i);
            }

            return panel;
        }
    }
}