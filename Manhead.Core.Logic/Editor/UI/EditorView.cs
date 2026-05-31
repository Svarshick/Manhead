using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Manhead.Core.Logic.Editor.Data;
using Microsoft.Xna.Framework;
using MonoGameGum;
using RenderingLibrary.Graphics;

namespace Manhead.Core.Logic.Editor.UI;

public class EditorView
{
    public static readonly Color PanelBgColor = new (24, 24, 28, 240);
    public static readonly Color ListBgColor = new (16, 16, 18, 255);
    public static readonly Color AccentGreen = new (46, 139, 87, 255);
    public static readonly Color AccentRed = new (178, 34, 34, 255);
    public static readonly Color AccentTabActive = new (45, 45, 52, 255);
    
    public EditorView(TemplateHolder templateHolder, EventBus eventBus)
    {
        var root = GumService.Default.Root;
        var leftPanel = new LeftPanel(templateHolder, eventBus)
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 20,
            Height = 100
        };

        var rightPanel = new RightPanel(eventBus)
        {
            XOrigin = HorizontalAlignment.Right,
            XUnits = GeneralUnitType.Percentage,
            X = 100,
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 20,
            Height = 100
        };
        root.AddChild(leftPanel);
        root.AddChild(CreateTopPanel());
        root.AddChild(rightPanel);
    }

    private Grid CreateTopPanel()
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

        for (int i = 0; i < 4; i++)
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