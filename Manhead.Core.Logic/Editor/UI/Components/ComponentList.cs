using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Manhead.Core.Logic.Editor.UI.Common;
using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using Microsoft.Xna.Framework;
using ObservableCollections;
using R3;

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
            FillColor = Color.Transparent;

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

            var componentView = ComponentViewFactory.Create(component, out _subscription);
            componentView.WidthUnits = DimensionUnitType.RelativeToParent;
            componentView.HeightUnits = DimensionUnitType.RelativeToChildren;
            componentView.Width = 0;
            componentView.Height = 0;
            stackPanel.AddChild(componentView);
    }
        
        public void Dispose()
        {
            _rmButton.Click -= RemoveIt;
            _subscription.Dispose();
        }

        private void RemoveIt(object? sender, EventArgs e)
        {
            _componentList._componentHolder.RemoveComponent(Component.GetType());
        }
    }

    private readonly IComponentHolder _componentHolder;
    private readonly ISynchronizedView<IComponent, ListElement> _componentsSyncView;

    private State _state;
    private enum State
    {
        List,
        Search,
    };
    
    private Grid _listGrid;
    private ListBox _listBox;
    private Button _addElementButton;
    
    private SearchDialog _searchDialog;
    
    private readonly CompositeDisposable _disposables = new();
    
    public ComponentList(IComponentHolder componentHolder, IReadOnlyList<Type> componentOptions)
    {
        _componentHolder = componentHolder;
        //List
        {
            _listGrid = new Grid
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
                RowDefinitions =
                {
                    new RowDefinition(new GridLength(30, GridUnitType.Absolute)),
                    new RowDefinition(new GridLength(1, GridUnitType.Star)),
                },
            };
            this.AddChild(_listGrid);

            //ScrollView
            {
                _listBox = new ListBox
                {
                    WidthUnits = DimensionUnitType.RelativeToParent,
                    HeightUnits = DimensionUnitType.RelativeToParent,
                    Width = 0,
                    Height = 0,
                };
                _listGrid.AddChild(_listBox, 1, 0);

                _componentsSyncView = _componentHolder.Components.CreateView(c => new ListElement(c, this));
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
                _addElementButton = new Button
                {
                    WidthUnits = DimensionUnitType.RelativeToParent,
                    HeightUnits = DimensionUnitType.RelativeToParent,
                    Width = 0,
                    Height = 0,
                    Text = "Add component"
                };
                _addElementButton.Click += (_, _) => ChangeState(State.Search);
                _listGrid.AddChild(_addElementButton, 0, 0);
            }
        }

        //SearchDialog
        {
            var options = componentOptions.Select(t => (t.Name, (object)t)).ToArray();
            _searchDialog = new SearchDialog(options)
            {
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
                Width = 0,
                Height = 0,
                Visible = false,
            };
            AddChild(_searchDialog);
            _searchDialog.Choose += (_, type) =>
            {
                ChangeState(State.List);
                var componentType = (Type)type;
                if (_componentHolder.HasComponent(componentType)) 
                    return;
                var component = (IComponent)Activator.CreateInstance(componentType)!;
                _componentHolder.AddComponent(component);
            };
            _searchDialog.RollOff += (_, _) => ChangeState(State.List);
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

    private void ChangeState(State next)
    {
        if (next == State.Search && _state == State.List)
        {
            _listGrid.IsVisible = false;
            _searchDialog.Visible = true;
            _searchDialog.SearchBox.Text = string.Empty;
            _searchDialog.FilterOptions();
            _searchDialog.SearchBox.IsFocused = true;
            _state = State.Search;
        }
        
        else if (next == State.List && _state == State.Search)
        {
            _listGrid.IsVisible = true;
            _searchDialog.Visible = false;
            _state = State.List;
        }
    }
    
    private void AddElement()
    {
        var component = _componentHolder.GetComponent<Visible>();
        if (component is null)
        {
            var visible = new Visible();
            _componentHolder.AddComponent(visible);
        }
    }
}