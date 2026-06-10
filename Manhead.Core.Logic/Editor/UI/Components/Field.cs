using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Wireframe;

namespace Manhead.Core.Logic.Editor.UI.Components;

public class Field : Grid
{
    public Field(string name, GraphicalUiElement editor)
    {
        WidthUnits = DimensionUnitType.RelativeToParent;
        Width = 0;
        HeightUnits = DimensionUnitType.RelativeToChildren;
        Height = 0;
            
        RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
        RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
        var label = new Label { Text = name };
        AddChild(label, 0, 0);
        AddChild(editor, 1, 0);
    }

    public Field(string name, FrameworkElement editor) : this(name, editor.Visual)
    {
    }
}