using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum;

namespace Manhead.Core.Logic.Editor.UI;

public class TabView : ContainerRuntime 
{
    private (Button TabButton, GraphicalUiElement TabContent)[] _tabs;
    
    public TabView((Button TabButton, GraphicalUiElement TabContent)[] tabs)
    {
        _tabs = tabs;
        
        var mainGrid = new Grid
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
            RowDefinitions =
            {
                new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                new RowDefinition(new GridLength(1, GridUnitType.Star)),
            }
        };
        this.AddChild(mainGrid);

        var tabsGrid = new Grid
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0,
        };
        mainGrid.AddChild(tabsGrid, 0, 0);
        
        for (int i = 0; i < _tabs.Length; i++)
        {
            tabsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        
            var button = _tabs[i].TabButton;
            var content = _tabs[i].TabContent;
            
            var number = i;
            button.Click += (_, _) => Select(number);
            tabsGrid.AddChild(button, 0, i);
            mainGrid.AddChild(content, 1, 0);
        }

        Select(0);
    }

    public void Select(int number)
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].TabButton.IsEnabled = i != number;
            _tabs[i].TabContent.Visible = i == number;
        }
    }
}

public static class TabViewTest 
{
    public static TabView GetTest()
    {
        var tabs = new (Button TabButton, GraphicalUiElement TabContent)[]
        {
            (new Button { Text = "1", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, new RectangleRuntime { FillColor = Color.Red }),
            (new Button { Text = "2", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, new RectangleRuntime { FillColor = Color.Green }),
            (new Button { Text = "3", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0}, new RectangleRuntime { FillColor = Color.Blue }),
            (new Button { Text = "4", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, new RectangleRuntime { FillColor = Color.White }),
            (new Button { Text = "5", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, new RectangleRuntime { FillColor = Color.Black }),
        };

        return new TabView(tabs);
    }
}