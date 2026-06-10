using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary.Graphics;

namespace Manhead.Core.Logic.Editor.UI;

public class RightPanel : ContainerRuntime
{
    public RightPanel(EventBus eventBus)
    {
        var mainGrid = new Grid
        {
            XOrigin = HorizontalAlignment.Right,
            XUnits = GeneralUnitType.PixelsFromLarge,
            X = 0,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(40, GridUnitType.Absolute)),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
            }
        };
        this.AddChild(mainGrid);

        var outerContentPanel = new RectangleRuntime
        {
            FillColor = EditorView.PanelBgColor,
            StrokeColor = EditorView.PanelBgColor,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };
        mainGrid.AddChild(outerContentPanel, 0, 1);

        var collapseButton = new Button
        {
            WidthUnits = DimensionUnitType.Absolute,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 40,
            Height = 0,
            Text = ">",
        };
        collapseButton.Click += (_, _) =>
        {
            if (mainGrid.WidthUnits == DimensionUnitType.RelativeToParent)
            {
                mainGrid.WidthUnits = DimensionUnitType.Absolute;
                mainGrid.Width = 40;
                outerContentPanel.Visible = false;
                collapseButton.Text = "<";
            }
            else
            {
                mainGrid.WidthUnits = DimensionUnitType.RelativeToParent;
                mainGrid.Width = 0;
                outerContentPanel.Visible = true;
                collapseButton.Text = ">";
            }
        };
        mainGrid.AddChild(collapseButton, 0, 0);

        var contentMargin = 15;
        var templateEditor = new TemplateEditor(eventBus)
        {
            XUnits = GeneralUnitType.PixelsFromSmall,
            YUnits = GeneralUnitType.PixelsFromSmall,
            X = contentMargin,
            Y = contentMargin,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = -30,
            Height = -30,
        };
        outerContentPanel.AddChild(templateEditor);
    }
}