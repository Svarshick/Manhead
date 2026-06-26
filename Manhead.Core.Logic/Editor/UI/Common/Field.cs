using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Wireframe;

namespace Manhead.Core.Logic.Editor.UI.Components;

public class Field : StackPanel
{
    public Field(string name, GraphicalUiElement editor)
    {
        WidthUnits = DimensionUnitType.RelativeToParent;
        Width = 0;
        HeightUnits = DimensionUnitType.RelativeToChildren;
        Height = 0;
        Orientation = Orientation.Vertical;
            
        var label = new Label { Text = name };
        AddChild(label);
        AddChild(editor);
    }

    public Field(string name, FrameworkElement editor) : this(name, editor.Visual)
    {
    }
}