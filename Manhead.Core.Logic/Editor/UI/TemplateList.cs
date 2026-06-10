using Gum.DataTypes;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Gameplay.Data;
using Microsoft.Xna.Framework;
using ObservableCollections;
using R3;

namespace Manhead.Core.Logic.Editor.UI;

public class TemplateList : ContainerRuntime, IDisposable
{
    private enum ListElementState
    {
        Unselected,
        Selected,
        Editing,
    }
    private class ListElement : RectangleRuntime, IDisposable
    {
        public readonly Template Template;

        private readonly EventBus _eventBus;
        private readonly ListBox _box;
        private readonly TextBox _textBox;
        private ListElementState _state = ListElementState.Unselected;
        
        private IDisposable _subscription;

        public ListElement(Template template, ListBox box, EventBus eventBus)
        {
            Template = template;
            _box = box;
            _eventBus = eventBus;

            WidthUnits = DimensionUnitType.RelativeToParent;
            HeightUnits = DimensionUnitType.RelativeToChildren;
            Width = 0;
            Height = 0;
            StrokeColor = Color.Transparent;
            
            _textBox = new TextBox();
            _textBox.Dock(Gum.Wireframe.Dock.FillHorizontally);
            _textBox.Text = template.Name;
            _subscription = template.Changed
                .Where(prop => prop == nameof(Template.Name))
                .Select(template, static (_, template) => template.Name)
                .Subscribe(_textBox, static (next, label) =>
                {
                    label.Text = next;
                });
            this.AddChild(_textBox);
            
            _textBox.GotFocus += GotFocus;
            _textBox.LostFocus += LostFocus;
            _eventBus.TemplateSelected += TemplateSelected;
        }
        
        public void Dispose()
        {
            _textBox.GotFocus -= GotFocus;
            _textBox.LostFocus -= LostFocus;
            _eventBus.TemplateSelected -= TemplateSelected;
            _subscription.Dispose();
        }

        private void TemplateSelected(Template? template)
        {
            if (Template != template)
                return;

            if (_box.SelectedObject != this)
            {
                var previous = _box.SelectedObject as ListElement;
                _box.SelectedObject = this;
                previous?.Refresh();
            }
            
            Refresh();
        }

        private void Refresh()
        {
            if (_box.SelectedObject != this)
            {
                SetUnselected();
            }
            else if (_textBox.IsFocused)
            {
                SetEditing();
            }
            else
            {
                SetSelected();
            }
        }

        private void GotFocus(object? sender, EventArgs e)
        {
            if (_state == ListElementState.Unselected)
            {
                _eventBus.SelectTemplate(Template);
            }
            else
            {
                Refresh();
            }
        }

        private void LostFocus(object? sender, EventArgs e)
        {
            Refresh();
        }
        
        private void SetSelected()
        {
            _state = ListElementState.Selected;
            StrokeColor = Color.CadetBlue;
        }

        private void SetUnselected()
        {
            _state = ListElementState.Unselected;    
            StrokeColor = Color.Transparent;
        }

        private void SetEditing()
        {
            _state = ListElementState.Editing;
            StrokeColor = Color.Coral;
        }
    }
    
    private readonly TemplateHolder _templateHolder;
    private readonly ISynchronizedView<KeyValuePair<Template, List<Placement>>, ListElement> _templatesSyncView;
    
    private readonly ListBox _listBox;
    private readonly Button _addButton;
    private readonly Button _rmButton;
    
    private readonly CompositeDisposable _disposables = new();

    public TemplateList(TemplateHolder templateHolder, EventBus eventBus)
    {
        _templateHolder = templateHolder;
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

            _templatesSyncView = _templateHolder.Templates.CreateView(kvp => new ListElement(kvp.Key, _listBox, eventBus));
            _disposables.Add(_templatesSyncView);

            var viewAdd = _templatesSyncView.ObserveAdd()
                .Subscribe(evt => _listBox.Items.Add(evt.Value.View));
            _disposables.Add(viewAdd);

            var viewRemove = _templatesSyncView.ObserveRemove()
                .Subscribe(evt =>
                {
                    var view = evt.Value.View;
                    view.Dispose();
                    _listBox.Items.Remove(view);
                    view.Dispose();
                });
            _disposables.Add(viewRemove);
        }
        
        //Buttons
        {
            var listButtons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent,
            };
            grid.AddChild(listButtons, 1, 0);

            _addButton = new Button
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 50,
                Height = 100,
                Text = "Add",
            };
            listButtons.AddChild(_addButton);

            _rmButton = new Button
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.PercentageOfParent,
                Width = 50,
                Height = 100,
                Text = "Remove",
            };
            listButtons.AddChild(_rmButton);

            _addButton.Click += AddTemplate;
            _rmButton.Click += RemoveTemplate;
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    private void AddTemplate(object? sender, EventArgs e)
    {
        var entity = new Entity();
        var template = new Template(entity);
        template.Name = "Template";
        _templateHolder.AddTemplate(template);
    }
    
    private void RemoveTemplate(object? sender, EventArgs e)
    {
    }
}