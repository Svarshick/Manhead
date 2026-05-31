using Gum.DataTypes;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using Microsoft.Xna.Framework;
using ObservableCollections;
using R3;
using MonoGameGum;

namespace Manhead.Core.Logic.Editor.UI.Components;

public class ComponentList : ContainerRuntime, IDisposable
{
    private class ListElement : RectangleRuntime, IDisposable
    {
        public readonly IComponent Component;
        private readonly ComponentList _componentList;

        private readonly Button _rmButton;
        private readonly GraphicalUiElement _componentVisual; 
        
        private IDisposable _subscription;

        public ListElement(IComponent component, ComponentList componentList)
        {
            Component = component;
            _componentList = componentList;
            
            WidthUnits = DimensionUnitType.RelativeToParent;
            HeightUnits = DimensionUnitType.RelativeToChildren;
            Width = 0;
            Height = 0;
            StrokeColor = Color.Transparent;
            FillColor  = Color.Transparent;

            var stackPanel = new StackPanel
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                Width = 0,
                Height = 0,
            };
            this.AddChild(stackPanel);

            var headerGrid = new Grid
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                Width = 0,
                Height = 0,
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                }
            };
            stackPanel.AddChild(headerGrid);
                
            var name = component.GetType().Name;
            var nameLabel = new Label
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                Width = 0,
                Text = name,
            };
            headerGrid.AddChild(nameLabel, 0, 0);

            _rmButton = new Button
            {
                WidthUnits = DimensionUnitType.Absolute,
                HeightUnits = DimensionUnitType.Absolute,
                Width = 30,
                Height = 30,
                Text = "X"
            };
            _rmButton.Click += RemoveIt;
            headerGrid.AddChild(_rmButton, 0, 1);

            var componentView = ComponentViewFabric.Create(component, out _subscription);
            stackPanel.AddChild(componentView);
        }
        
        public void Dispose()
        {
            _rmButton.Click -= RemoveIt;
            _subscription.Dispose();
        }

        private void RemoveIt(object? sender, EventArgs e)
        {
            _componentList._entity.RemoveComponent(Component.GetType());
        }
    }

    private readonly Entity _entity;
    private readonly ISynchronizedView<IComponent, ListElement> _componentsSyncView;

    private ListBox _listBox;
    private Button _addVisibleButton;

    private readonly CompositeDisposable _disposables = new();
    
    public ComponentList(Entity entity)
    {
        _entity = entity;
        var grid = new Grid
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            RowDefinitions =
            {
                new RowDefinition(new GridLength(1, GridUnitType.Star)),
                new RowDefinition(new GridLength(30, GridUnitType.Absolute)),
            },
        };
        this.AddChild(grid);

        //ScrollView
        {
            _listBox = new ListBox
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
                Width = 0,
                Height = 0,
                VisualTemplate = new VisualTemplate(item => ((ListElement)item)),
            };
            grid.AddChild(_listBox, 0, 0);

            _componentsSyncView = _entity.Components.CreateView(c => new ListElement(c, this));
            _disposables.Add(_componentsSyncView);

            var viewAdd = _componentsSyncView.ObserveAdd()
                .Subscribe(evt => _listBox.Items.Add(evt.Value.View));
            _disposables.Add(viewAdd);

            var viewRemove = _componentsSyncView.ObserveRemove()
                .Subscribe(evt =>
                {
                    var view = evt.Value.View;
                    view.Dispose();
                    _listBox.Items.Remove(view);
                    view.Dispose();
                });
            _disposables.Add(viewRemove);

            foreach (var view in _componentsSyncView)
            {
                _listBox.Items.Add(view);
            }
        }

        //Buttons
        {
            _addVisibleButton = new Button
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
                Width = 0,
                Height = 0,
                Text = "Add Visible"
            };
            grid.AddChild(_addVisibleButton, 1, 0);
            _addVisibleButton.Click += AddVisible;
        }
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
        foreach (var item in _listBox.Items)
        {
            var le = (ListElement)item;
            le.Dispose();
        }
    }

    private void AddVisible(object? sender, EventArgs e)
    {
        var component = _entity.GetComponent<Visible>();
        if (component is null)
        {
            var visible = new Visible();
            _entity.AddComponent(visible);
        }
    }
}