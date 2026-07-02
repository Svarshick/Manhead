using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Manhead.Core.Logic;

public class ScreenLayout
{
    public int WidthResolution = 1920;
    public int HeightResolution = 1080;
    public float PixelsPerUnit = 100f;

    public readonly OrthographicCamera Camera;

    public ScreenLayout(GraphicsDevice graphicsDevice)
    {
        var viewportAdapter = new DefaultViewportAdapter(graphicsDevice);
        Camera = new OrthographicCamera(viewportAdapter);
    }

    public float ToPixels(float units) => units * PixelsPerUnit;
    public Vector2 ToPixels(float x, float y) => new Vector2(x * PixelsPerUnit, y * PixelsPerUnit);
    public Vector2 ToPixels(Vector2 units) => units * PixelsPerUnit;
    public float ToUnits(float pixels) => pixels / PixelsPerUnit;
    public Vector2 ToUnits(Vector2 pixels) => pixels / PixelsPerUnit;

    public Vector2 CameraCenter() => Camera.Center;
    public Vector2 CameraTopLeft() => Camera.BoundingRectangle.TopLeft;
    public Vector2 CameraTopRight() => Camera.BoundingRectangle.TopRight;
    public Vector2 CameraBottomLeft() => Camera.BoundingRectangle.BottomLeft;
    public Vector2 CameraBottomRight() => Camera.BoundingRectangle.BottomRight;
    
    public void FollowPosition(Vector2 position) => Camera.LookAt(position);
}