using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Manhead.Core.Logic.Editor;

public class GridView : IDrawable
{
    private readonly Texture2D _blankTexture;
    private readonly GridLayout _gridLayout;
    private readonly Effect _gridEffect;

    public int Width;
    public int Height;
    
    public GridView(GraphicsDevice graphicsDevice, GridLayout gridLayout)
    {
        _gridLayout = gridLayout;
        _blankTexture = new Texture2D(graphicsDevice, 1, 1);
        _blankTexture.SetData(new[] { Color.White });
        _gridEffect = ManheadGame.Content.Load<Effect>("Content/GridShader");
    }
    
    public void Draw()
    {
        ManheadGame.SpriteBatch.End();

        float zoom = ManheadGame.ScreenLayout.Camera.Zoom;
        Vector2 cellSizeInPixels = _gridLayout.CellSize * zoom;

        _gridEffect.Parameters["GridSize"]?.SetValue(new Vector2(Width, Height));
        _gridEffect.Parameters["LineColor"]?.SetValue(new Vector4(1f, 1f, 1f, 0.25f));
        _gridEffect.Parameters["CellSizeInPixels"]?.SetValue(cellSizeInPixels);

        ManheadGame.SpriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.LinearClamp,
            rasterizerState: RasterizerState.CullNone,
            effect: _gridEffect,
            transformMatrix: ManheadGame.ScreenLayout.Camera.GetViewMatrix()
        );

        float fieldWidth = Width * _gridLayout.CellSize.X;
        float fieldHeight = Height * _gridLayout.CellSize.Y;

        ManheadGame.SpriteBatch.Draw(
            _blankTexture,
            Vector2.Zero,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            new Vector2(fieldWidth, fieldHeight), // No division needed since texture size is 1x1
            SpriteEffects.None,
            0f
        );
        
        ManheadGame.SpriteBatch.End();

        ManheadGame.SpriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.AlphaBlend,
            rasterizerState: RasterizerState.CullNone,
            transformMatrix: ManheadGame.ScreenLayout.Camera.GetViewMatrix()
        );
    }
}