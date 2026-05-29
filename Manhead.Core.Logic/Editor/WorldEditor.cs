using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using Manhead.Core.Logic.Gameplay.View;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Manhead.Core.Logic.Editor;

public class WorldEditor : IDrawable
{
    private readonly Input _input;
    private readonly GridLayout _gridLayout;
    private readonly Field<Placement> _field;
    private readonly GridView _gridView;
    private readonly TemplatesHolder _templatesHolder;
    private readonly Template _template;
    
    private readonly EditorUi _ui;
    
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
        _field = new(width, height);
        _gridView = new(graphicsDevice, gridLayout);
        _gridView.Width = width;
        _gridView.Height = height;
        
        var viewBuilder = new ViewBuilder(_gridLayout);
        var viewHolder = new ViewHolder(viewBuilder, _gridLayout, _field.Width, _field.Height);
        _templatesHolder = new(viewHolder);
        var entity = new Entity();
        var visibleComponent = new Visible();
        visibleComponent.Color = Color.Red;
        entity.AddComponent(visibleComponent);
        _template = new Template(entity);
        _templatesHolder.AddTemplate(_template);
        
        _ui = new EditorUi();
        
        SquareTexture = Game.Content.Load<Texture2D>("Content/Square");
       
        _input.Put += OnPut;
        _input.Remove += OnRemove;
        _input.UpdateDrag += OnUpdateDrag;
        _input.Zoom += OnZoom;
    }
    
    private void OnPut(Vector2 mousePosition)
    {
        var worldPosition = Game.ScreenLayout.Camera.ScreenToWorld(mousePosition);
        var gridPosition = _gridLayout.WorldToGrid(worldPosition);
        if (!_field.Has(gridPosition) || _field[gridPosition].Count > 0)
            return;

        var placement = new Placement(null, gridPosition);
        _field[gridPosition].Add(placement);
        _templatesHolder.AddPlacement(_template, gridPosition);
        /*
        var appearance = new RectangleView();
        appearance.Width = _gridLayout.CellSize.X;
        appearance.Height = _gridLayout.CellSize.Y;
        appearance.Color = Color.White;
        appearance.RelativePosition = _gridLayout.GridToWorld(gridPosition);
        _views.Add(appearance);*/
    }

    private void OnRemove(Vector2 mousePosition)
    {
        var worldPosition = Game.ScreenLayout.Camera.ScreenToWorld(mousePosition);
        var gridPosition = _gridLayout.WorldToGrid(worldPosition);
        if (!_field.Has(gridPosition))
            return;

        _field[gridPosition].Clear();
        _templatesHolder.RemovePlacement(_template, gridPosition);
    }

    private void OnZoom(float deltaZoom)
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

    private void OnUpdateDrag(Vector2 mousePosition, Vector2 delta)
    {
        var zoom = Game.ScreenLayout.Camera.Zoom;
        var worldDelta = delta / zoom;
        Game.ScreenLayout.Camera.Position -= worldDelta;
    }

    public void Draw()
    {
        _gridView.Draw();
        _templatesHolder.View.Draw();
    }
}