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
        var buttonWidth = 40;
        var stackPanel = new StackPanel
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
            Orientation = Orientation.Horizontal,
        };
        this.AddChild(stackPanel);

        var outerContentPanel = new RectangleRuntime
        {
            FillColor = EditorUI.PanelBgColor,
            IsFilled = true,
            StrokeColor = EditorUI.PanelBgColor,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = -buttonWidth,
            Height = 0
        };

        var collapseButton = new Button
        {
            WidthUnits = DimensionUnitType.Absolute,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = buttonWidth,
            Height = 0,
            Text = "<",
        };
        
        collapseButton.Click += (_, _) =>
        {
            if (stackPanel.WidthUnits == DimensionUnitType.RelativeToParent)
            {
                stackPanel.WidthUnits = DimensionUnitType.Absolute;
                stackPanel.Width = 40;
                outerContentPanel.Visible = false;
                collapseButton.Text = ">";
            }
            else
            {
                stackPanel.WidthUnits = DimensionUnitType.RelativeToParent;
                stackPanel.Width = 0;
                outerContentPanel.Visible = true;
                collapseButton.Text = "<";
            }
        };
        
        stackPanel.AddChild(outerContentPanel);
        stackPanel.AddChild(collapseButton);

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