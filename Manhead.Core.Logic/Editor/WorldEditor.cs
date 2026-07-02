using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Manhead.Core.Logic.Editor;

public class WorldEditor : IDrawable, IDisposable
{
    private readonly EditorInput _editorInput;
    private readonly GridLayout _gridLayout;
    private readonly GridView _gridView;
    private readonly TemplateHolder _templateHolder;
    private readonly EventBus _eventBus;
    
    private Template? _selectedTemplate;
    
    public readonly Texture2D SquareTexture;
    
    private float _minZoom = 0.1f;
    private float _maxZoom = 2f;

    
    public WorldEditor(
        EditorInput editorInput,
        EventBus eventBus,
        TemplateHolder templateHolder,
        GridLayout gridLayout,
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        _editorInput = editorInput;
        _eventBus = eventBus;
        _templateHolder = templateHolder;
        _gridLayout = gridLayout; 
        _gridView = new(gridLayout, graphicsDevice, contentManager);
        _gridView.Width = _templateHolder.Placements.Width;
        _gridView.Height = _templateHolder.Placements.Height;

        SquareTexture = contentManager.Load<Texture2D>("Content/Square");
       
        _editorInput.Draw += Draw;
        _editorInput.Erase += Remove;
        _editorInput.UpdateDrag += MoveCamera;
        _editorInput.Zoom += Zoom;
        _eventBus.SelectTemplate += SelectTemplate;
    }

    public void Dispose()
    {
        _editorInput.Draw -= Draw;
        _editorInput.Erase -= Remove;
        _editorInput.UpdateDrag -= MoveCamera;
        _editorInput.Zoom -= Zoom;
        _eventBus.SelectTemplate -= SelectTemplate;
    }
    
    private void Draw(Vector2 mousePosition)
    {
        var worldPosition = ManheadGame.ScreenLayout.Camera.ScreenToWorld(mousePosition);
        var gridPosition = _gridLayout.WorldToGrid(worldPosition);
        if (_templateHolder.IsFilled(gridPosition))
        {
            var placement = _templateHolder.GetPlacement(gridPosition);
            _eventBus.RaiseSelectTemplate(placement.Template);
            return;
        }
        
        if (_selectedTemplate is null ||
            !_templateHolder.IsFree(gridPosition))
            return;

        _templateHolder.AddPlacement(_selectedTemplate, gridPosition);
    }

    private void Remove(Vector2 mousePosition)
    {
        var worldPosition = ManheadGame.ScreenLayout.Camera.ScreenToWorld(mousePosition);
        var gridPosition = _gridLayout.WorldToGrid(worldPosition);
        if (!_templateHolder.IsFilled(gridPosition))
            return;
        
        _templateHolder.RemovePlacement(gridPosition);
    }

    private void Zoom(float deltaZoom)
    {
        var zoom = ManheadGame.ScreenLayout.Camera.Zoom;
        zoom -= deltaZoom;
        if (zoom < _minZoom)
        {
            zoom = _minZoom;
        }

        if (zoom > _maxZoom)
        {
            zoom = _maxZoom;
        }

        ManheadGame.ScreenLayout.Camera.Zoom = zoom;
    }

    private void MoveCamera(Vector2 mousePosition, Vector2 delta)
    {
        var zoom = ManheadGame.ScreenLayout.Camera.Zoom;
        var worldDelta = delta / zoom;
        ManheadGame.ScreenLayout.Camera.Position -= worldDelta;
    }

    private void SelectTemplate(Template? template)
    {
        _selectedTemplate = template;
    }

    public void Draw()
    {
        _gridView.Draw();
        _templateHolder.TemplatesView.Draw();
    }
}