using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;

namespace Manhead.Core.Logic.Editor;

public class EditorUi
{
    public readonly Color PanelBgColor = new (24, 24, 28, 240);
    public readonly Color ListBgColor = new (16, 16, 18, 255);
    public readonly Color AccentGreen = new (46, 139, 87, 255);
    public readonly Color AccentRed = new (178, 34, 34, 255);
    public readonly Color AccentTabActive = new (45, 45, 52, 255);
    
    public EditorUi()
    {
        var root = GumService.Default.Root;
        root.AddChild(CreateLeftPanel());
        root.AddChild(CreateTopPanel());
        root.AddChild(CreateRightPanel());
    }

    private Grid CreateLeftPanel()
    {
        var panel = new Grid
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 20,
            Height = 100,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(40, GridUnitType.Absolute)),
            }
        };
        
        var outerContentPanel = new ContainerRuntime
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 100,
            Height = 100,
            X = 0,
            Y = 0
        };
        panel.AddChild(outerContentPanel, 0, 0);

        var collapseButton = new Button
        {
            WidthUnits = DimensionUnitType.Absolute,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 40,
            Height = 100,
            Text = "<",
        };
        collapseButton.Click += (_, _) =>
        {
            if (panel.WidthUnits == DimensionUnitType.PercentageOfParent)
            {
                panel.WidthUnits = DimensionUnitType.Absolute;
                panel.Width = 40;
                outerContentPanel.Visible = false;
                collapseButton.Text = ">";
            }
            else
            {
                panel.WidthUnits = DimensionUnitType.PercentageOfParent;
                panel.Width = 20;
                outerContentPanel.Visible = true;
                collapseButton.Text = "<";
            }
        };
        panel.AddChild(collapseButton, 0, 1);
            
        var outerContentBackground = new ColoredRectangleRuntime
        {
            Color = PanelBgColor,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0
        };
        outerContentPanel.AddChild(outerContentBackground);

        var contentMargin = 15;
        var innerContentPanel = new StackPanel
        {
            XUnits = GeneralUnitType.PixelsFromSmall,
            YUnits = GeneralUnitType.PixelsFromSmall,
            X = contentMargin,
            Y = contentMargin,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = -30,
            Height = -30
        };
        outerContentPanel.AddChild(innerContentPanel);

        var list = new ScrollViewer
        {
            XUnits = GeneralUnitType.PixelsFromSmall,
            YUnits = GeneralUnitType.PixelsFromSmall,
            X = 0,
            Y = 0,
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 100,
            Height = 80
        };
        for (int i = 0; i < 50; i++)
        {
            var button = new Button
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                Width = 100,
                Text = "Button " + i
            };
            button.Click += (_, _) =>
                button.Text = DateTime.Now.ToString();
            list.AddChild(button);
        }

        innerContentPanel.AddChild(list);

        var listButtons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            XUnits = GeneralUnitType.PixelsFromSmall,
            YUnits = GeneralUnitType.PixelsFromSmall,
            X = 0,
            Y = 15,
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 100,
            Height = 10
        };
        innerContentPanel.AddChild(listButtons);

        var addButton = new Button
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 50,
            Height = 100,
            Text = "Add"
        };
        listButtons.AddChild(addButton);

        var rmButton = new Button
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 50,
            Height = 100,
            Text = "Remove"
        };
        listButtons.AddChild(rmButton);

        return panel;
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
    
    private Grid CreateRightPanel()
    {
        var panel = new Grid
        {
            XOrigin = HorizontalAlignment.Right,
            XUnits = GeneralUnitType.Percentage,
            X = 100,
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 20,
            Height = 100,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(40, GridUnitType.Absolute)),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
            }
        };

        var contentMargin = 15;
        var contentPanel = CreateContentPanel();

        var parametersContainer = new ContainerRuntime
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 100,
            Height = 100,
        };
        contentPanel.AddChild(parametersContainer, 1, 0);
        var parameterGrids = new Grid[5];
        for (int i = 0; i < 5; i++)
        {
            var grid = new Grid
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                Height = 100,
                RowDefinitions =
                {
                    new RowDefinition(new GridLength(1, GridUnitType.Star)),
                    new RowDefinition(new GridLength(40, GridUnitType.Absolute)),
                }
            };

            parameterGrids[i] = grid;
            parametersContainer.AddChild(grid);
            
            var list = new ScrollViewer
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                Height = 100
            };
            for (int j = 0; j < 50; j++)
            {
                var button = new Button
                {
                    WidthUnits = DimensionUnitType.RelativeToParent,
                    Width = 100,
                    Text = "List" + i + "::Button " + j
                };
                button.Click += (_, _) =>
                    button.Text = DateTime.Now.ToString();
                list.AddChild(button);
            }

            grid.AddChild(list, 0, 0);

            var listButtons = new StackPanel
            {
                YOrigin = VerticalAlignment.Top,
                YUnits = GeneralUnitType.PixelsFromSmall,
                Y = 15,
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
                Width = 0,
                Height = -15,
                Orientation = Orientation.Horizontal,
            };
            grid.AddChild(listButtons, 1, 0);

            var addButton = new Button
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 50,
                Height = 100,
                Text = "Add"
            };
            listButtons.AddChild(addButton);

            var rmButton = new Button
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 50,
                Height = 100,
                Text = "Remove"
            };
            listButtons.AddChild(rmButton);
        }
        
        var tabPanel = CreateTabPanel();
        contentPanel.AddChild(tabPanel, 0, 0);

        return panel;

        Grid CreateContentPanel()
        {
            var outerContentPanel = new ContainerRuntime
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                Height = 100,
                X = 0,
                Y = 0
            };
            panel.AddChild(outerContentPanel, 0, 1);

            var collapseButton = new Button
            {
                WidthUnits = DimensionUnitType.Absolute,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 40,
                Height = 100,
                Text = ">",
            };
            collapseButton.Click += (_, _) =>
            {
                if (panel.WidthUnits == DimensionUnitType.PercentageOfParent)
                {
                    panel.WidthUnits = DimensionUnitType.Absolute;
                    panel.Width = 40;
                    outerContentPanel.Visible = false;
                    collapseButton.Text = "<";
                }
                else
                {
                    panel.WidthUnits = DimensionUnitType.PercentageOfParent;
                    panel.Width = 20;
                    outerContentPanel.Visible = true;
                    collapseButton.Text = ">";
                }
            };
            panel.AddChild(collapseButton, 0, 0);

            var outerContentBackground = new ColoredRectangleRuntime
            {
                Color = PanelBgColor,
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
                Width = 0,
                Height = 0
            };
            outerContentPanel.AddChild(outerContentBackground);

            var innerContentPanel = new Grid
            {
                XUnits = GeneralUnitType.PixelsFromSmall,
                YUnits = GeneralUnitType.PixelsFromSmall,
                X = contentMargin,
                Y = contentMargin,
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
                Width = -30,
                Height = -30,
                RowDefinitions =
                {
                    new RowDefinition(new GridLength(40, GridUnitType.Absolute)),
                    new RowDefinition(new GridLength(1, GridUnitType.Star)),
                }
            };
            
            outerContentPanel.AddChild(innerContentPanel);
            
            return innerContentPanel;
        }

        Grid CreateTabPanel()
        {
            var tabPanel = new Grid
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                Height = 100,
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                }
            };

            for (int i = 0; i < parameterGrids.Length; i++)
            {
                var button = new Button
                {
                    WidthUnits = DimensionUnitType.PercentageOfParent,
                    HeightUnits = DimensionUnitType.PercentageOfParent,
                    Width = 100,
                    Height = 100,
                    Text = $"Parameter {i}"
                };
                var number = i;
                button.Click += (_, _) =>
                {
                    for (int j = 0; j < parameterGrids.Length; j++)
                    {
                        parameterGrids[j].IsVisible = j == number;
                    }
                };
                tabPanel.AddChild(button, 0, i);
            }
            
            return tabPanel;
        }
    }
}