using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using RenderingLibrary.Graphics;

namespace Manhead.Core.Logic.Editor.UI;

public class RightPanel : ContainerRuntime
{
    public RightPanel(EventBus eventBus)
    {
        var buttonWidth = 40;
        var stackPanel = new StackPanel
        {
            XOrigin = HorizontalAlignment.Right,
            XUnits = GeneralUnitType.PixelsFromLarge,
            X = 0,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
            Orientation = Orientation.Horizontal,
        };
        this.AddChild(stackPanel);

        var collapseButton = new Button
        {
            WidthUnits = DimensionUnitType.Absolute,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = buttonWidth,
            Height = 0,
            Text = ">",
        };
        
        var outerContentPanel = new RectangleRuntime
        {
            FillColor = EditorUI.PanelBgColor,
            IsFilled = true,
            StrokeColor = EditorUI.PanelBgColor,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = -buttonWidth,
            Height = 0,
        };
        
        collapseButton.Click += (_, _) =>
        {
            if (stackPanel.WidthUnits == DimensionUnitType.RelativeToParent)
            {
                stackPanel.WidthUnits = DimensionUnitType.Absolute;
                stackPanel.Width = 40;
                outerContentPanel.Visible = false;
                collapseButton.Text = "<";
            }
            else
            {
                stackPanel.WidthUnits = DimensionUnitType.RelativeToParent;
                stackPanel.Width = 0;
                outerContentPanel.Visible = true;
                collapseButton.Text = ">";
            }
        };
        
        stackPanel.AddChild(collapseButton);
        stackPanel.AddChild(outerContentPanel);

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