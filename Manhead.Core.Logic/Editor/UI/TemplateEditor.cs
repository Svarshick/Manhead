using System.Linq.Expressions;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Editor.UI.Common;
using Manhead.Core.Logic.Editor.UI.Components;
using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using R3;

namespace Manhead.Core.Logic.Editor.UI;

public class TemplateEditor : RectangleRuntime
{
    private Template? _currentTemplate;
    private TabView? _currentTabView;
    private Label? _templateNotSelected;
    
    private CompositeDisposable? _disposables;
    
    public TemplateEditor(EventBus eventBus)
    {
        _currentTemplate = null;
        _currentTabView = null;
        _disposables = null;
        _templateNotSelected = CreateTemplateNotSelected();
        this.AddChild(_templateNotSelected);
        
        eventBus.TemplateSelected += TemplateSelected;
    }

    private void TemplateSelected(Template? template)
    {
        if (_currentTemplate == template)
            return;
        
        _currentTemplate = template;
        _disposables?.Dispose();
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
        
        _disposables = new CompositeDisposable();

        var selfComponents = new ComponentList(template.Entity, ComponentRegistry.EntityComponents)
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };
        
        var leftSideComponents = new ComponentList(template.Entity.LeftSide, ComponentRegistry.SideComponents)
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };

        var rightSideComponents = new ComponentList(template.Entity.RightSide, ComponentRegistry.SideComponents)
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };

        var frontSideComponents = new ComponentList(template.Entity.FrontSide, ComponentRegistry.SideComponents)
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };
        
        var backSideComponents = new ComponentList(template.Entity.BackSide, ComponentRegistry.SideComponents)
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };
        
        var tabs = new (Button TabButton, GraphicalUiElement TabContent)[]
        {
            (new Button { Text = "Self", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, selfComponents),
            (new Button { Text = "Left", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, leftSideComponents),
            (new Button { Text = "Right", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0}, rightSideComponents),
            (new Button { Text = "Front", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, frontSideComponents),
            (new Button { Text = "Back", WidthUnits = DimensionUnitType.RelativeToParent, Width = 0 }, backSideComponents),
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