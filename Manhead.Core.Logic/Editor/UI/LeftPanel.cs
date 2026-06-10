using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Manhead.Core.Logic.Editor.Data;

namespace Manhead.Core.Logic.Editor.UI;

public class LeftPanel : ContainerRuntime
{
    public LeftPanel(TemplateHolder templateHolder, EventBus eventBus)
    {
        var mainGrid = new Grid
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(40, GridUnitType.Absolute)),
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
            Height = 0
        };
        mainGrid.AddChild(outerContentPanel, 0, 0);

        var collapseButton = new Button
        {
            WidthUnits = DimensionUnitType.Absolute,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 40,
            Height = 0,
            Text = "<",
        };
        collapseButton.Click += (_, _) =>
        {
            if (mainGrid.WidthUnits == DimensionUnitType.RelativeToParent)
            {
                mainGrid.WidthUnits = DimensionUnitType.Absolute;
                mainGrid.Width = 40;
                outerContentPanel.Visible = false;
                collapseButton.Text = ">";
            }
            else
            {
                mainGrid.WidthUnits = DimensionUnitType.RelativeToParent;
                mainGrid.Width = 0;
                outerContentPanel.Visible = true;
                collapseButton.Text = "<";
            }
        };
        mainGrid.AddChild(collapseButton, 0, 1);

        var contentMargin = 15;
        var templateList = new TemplateList(templateHolder, eventBus)
        {
            XUnits = GeneralUnitType.PixelsFromSmall,
            YUnits = GeneralUnitType.PixelsFromSmall,
            X = contentMargin,
            Y = contentMargin,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = -contentMargin * 2,
            Height = -contentMargin * 2,
        };
        outerContentPanel.AddChild(templateList);
    }
}