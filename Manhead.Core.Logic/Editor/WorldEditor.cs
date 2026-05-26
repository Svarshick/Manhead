using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.View;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Manhead.Core.Logic.Editor;

public class WorldEditor : IDrawable
{
    private readonly Input _input;
    private readonly GridLayout _gridLayout;
    private readonly Field<EntityPlacement> _field = new(100, 100);
    private readonly List<View> _views = new();

    public readonly Texture2D SquareTexture;
    private readonly Effect _gridEffect;
    
    private readonly EditorUi _ui;
    
    private float _minZoom = 0.1f;
    private float _maxZoom = 2f;
    
    public WorldEditor(Input input, GridLayout gridLayout)
    {
        _input = input;
        _gridLayout = gridLayout;
        SquareTexture = Game.Content.Load<Texture2D>("Content/Square");
        _gridEffect = Game.Content.Load<Effect>("Content/GridShader");
        
        _ui = new EditorUi();
       
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

        var placement = new EntityPlacement(null, gridPosition);
        _field[gridPosition].Add(placement);

        var appearance = new View();
        appearance.Position = _gridLayout.GridRectangle(gridPosition).TopLeft;
        appearance.Sprite = new Sprite(SquareTexture);
        appearance.Sprite.Color = Random.Shared.Color(); 
        _views.Add(appearance);
    }

    private void OnRemove(Vector2 mousePosition)
    {
        var worldPosition = Game.ScreenLayout.Camera.ScreenToWorld(mousePosition);
        var gridPosition = _gridLayout.WorldToGrid(worldPosition);
        if (!_field.Has(gridPosition))
            return;

        _field[gridPosition].Clear();

        var cellTopLeft = _gridLayout.GridRectangle(gridPosition).TopLeft;
        _views.RemoveAll(v => Vector2.Distance(v.Position, cellTopLeft) < 0.01f);
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
        DrawGrid();
        var squareScaleF = Game.ScreenLayout.PixelsPerUnit / 1000;
        var squareScale = new Vector2(squareScaleF, squareScaleF);
        foreach (var view in _views)
        {
            Game.SpriteBatch.Draw(view.Sprite, view.WorldPosition, 0, squareScale);
        }
    }

    private void DrawGrid()
    {
        Game.SpriteBatch.End();

        float zoom = Game.ScreenLayout.Camera.Zoom;
        Vector2 cellSizeInPixels = _gridLayout.CellSize * zoom;

        _gridEffect.Parameters["GridSize"]?.SetValue(new Vector2(_field.Width, _field.Height));
        _gridEffect.Parameters["LineColor"]?.SetValue(new Vector4(1f, 1f, 1f, 0.25f));
        _gridEffect.Parameters["CellSizeInPixels"]?.SetValue(cellSizeInPixels);

        Game.SpriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.LinearClamp,
            rasterizerState: RasterizerState.CullNone,
            effect: _gridEffect,
            transformMatrix: Game.ScreenLayout.Camera.GetViewMatrix()
        );

        float fieldWidth = _field.Width * _gridLayout.CellSize.X;
        float fieldHeight = _field.Height * _gridLayout.CellSize.Y;

        Game.SpriteBatch.Draw(
            SquareTexture,
            Vector2.Zero,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            new Vector2(fieldWidth / SquareTexture.Width, fieldHeight / SquareTexture.Height),
            SpriteEffects.None,
            0f
        );

        Game.SpriteBatch.End();

        Game.SpriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            rasterizerState: RasterizerState.CullNone,
            transformMatrix: Game.ScreenLayout.Camera.GetViewMatrix()
        );
    }
}