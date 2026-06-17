using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Editor.UI;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Manhead.Core.Logic.Editor;

public class WorldEditor : IDrawable
{
    private readonly Input _input;
    private readonly GridLayout _gridLayout;
    private readonly GridView _gridView;
    private readonly TemplateHolder _templateHolder;
    
    private readonly EventBus _eventBus;
    private readonly EditorView _view;
    private Template? _selectedTemplate;
    
    public readonly Texture2D SquareTexture;
    
    private float _minZoom = 0.1f;
    private float _maxZoom = 2f;

    
    public WorldEditor(
        Input input, 
        GraphicsDevice graphicsDevice, 
        GridLayout gridLayout,
        int width = 100,
        int height = 100)
    {
        _input = input;
        _gridLayout = gridLayout;
        _gridView = new(graphicsDevice, gridLayout);
        _gridView.Width = width;
        _gridView.Height = height;
       _templateHolder = new (_gridLayout, width, height);
       
        _eventBus = new EventBus();
        _view = new EditorView(_templateHolder, _eventBus);
        
        SquareTexture = Game.Content.Load<Texture2D>("Content/Square");
       
        _input.Draw += Draw;
        _input.Erase += Remove;
        _input.UpdateDrag += MoveCamera;
        _input.Zoom += Zoom;
        _eventBus.TemplateSelected += template => _selectedTemplate = template;
    }
    
    private void Draw(Vector2 mousePosition)
    {
        var worldPosition = Game.ScreenLayout.Camera.ScreenToWorld(mousePosition);
        var gridPosition = _gridLayout.WorldToGrid(worldPosition);
        if (_templateHolder.IsFilled(gridPosition))
        {
            var placement = _templateHolder.GetPlacement(gridPosition);
            _eventBus.SelectTemplate(placement.Template);
            return;
        }
        
        if (_selectedTemplate is null ||
            !_templateHolder.IsFree(gridPosition))
            return;

        _templateHolder.AddPlacement(_selectedTemplate, gridPosition);
    }

    private void Remove(Vector2 mousePosition)
    {
        var worldPosition = Game.ScreenLayout.Camera.ScreenToWorld(mousePosition);
        var gridPosition = _gridLayout.WorldToGrid(worldPosition);
        if (!_templateHolder.IsFilled(gridPosition))
            return;
        
        _templateHolder.RemovePlacement(gridPosition);
    }

    private void Zoom(float deltaZoom)
    {
        var zoom = Game.ScreenLayout.Camera.Zoom;
        zoom -= deltaZoom;
        if (zoom < _minZoom)
        {
            zoom = _minZoom;
        }

        if (zoom > _maxZoom)
        {
            zoom = _maxZoom;
        }

        Game.ScreenLayout.Camera.Zoom = zoom;
    }

    private void MoveCamera(Vector2 mousePosition, Vector2 delta)
    {
        var zoom = Game.ScreenLayout.Camera.Zoom;
        var worldDelta = delta / zoom;
        Game.ScreenLayout.Camera.Position -= worldDelta;
    }

    public void Draw()
    {
        _gridView.Draw();
        _templateHolder.TemplatesView.Draw();
    }
}