using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Editor.UI.Components;
using Microsoft.Xna.Framework;
using MonoGameGum;

namespace Manhead.Core.Logic.Editor.UI;

public class TemplateEditor : RectangleRuntime
{
    private Template? _currentTemplate;
    private TabView? _currentTabView;
    private Label? _templateNotSelected;
    
    private IDisposable? _disposable;
    
    public TemplateEditor(EventBus eventBus)
    {
        _currentTemplate = null;
        _currentTabView = null;
        _disposable = null;
        _templateNotSelected = CreateTemplateNotSelected();
        this.AddChild(_templateNotSelected);
        
        eventBus.TemplateSelected += TemplateSelected;
    }

    private void TemplateSelected(Template? template)
    {
        if (_currentTemplate == template)
            return;
        
        _currentTemplate = template;
        _disposable?.Dispose();
        if (_currentTabView is not null)
        {
            RemoveChild(_currentTabView);
            _currentTabView = null;
        }

        if (_templateNotSelected is not null)
        {
            RemoveChild(_templateNotSelected.Visual);
            _templateNotSelected = null;
        }
        

        if (template is null)
        {
            _templateNotSelected = CreateTemplateNotSelected();
            this.AddChild(_templateNotSelected);
            return;
        }
        
        var components = new ComponentList(template.Entity)
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };
        _disposable = components;
        
        var tabs = new (Button TabButton, GraphicalUiElement TabContent)[]
        {
            (new Button { Text = "Self", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, components),
            (new Button { Text = "Left", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, new RectangleRuntime { FillColor = Color.Green }),
            (new Button { Text = "Right", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0}, new RectangleRuntime { FillColor = Color.Blue }),
            (new Button { Text = "Front", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, new RectangleRuntime { FillColor = Color.White }),
            (new Button { Text = "Back", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, new RectangleRuntime { FillColor = Color.Black }),
        };

        _currentTabView = new TabView(tabs)
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };
        AddChild(_currentTabView);
    }

    private Label CreateTemplateNotSelected()
    {
        var label = new Label
        {
            Text = "No template selected",
        };
        label.Dock(Gum.Wireframe.Dock.Fill);
        return label;
    }
}