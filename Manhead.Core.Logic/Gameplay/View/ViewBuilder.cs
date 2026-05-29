using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;

namespace Manhead.Core.Logic.Gameplay.View;

public abstract class View : IDrawable
{
    private readonly List<View> _children = new();

    public View? Parent
    {
        get;
        private set
        {
            field = value;
            RecalculatePosition();
            RecalculateDepth();
        }
    }
    
    public Vector2 RelativePosition
    {
        get;
        set
        {
            field = value;
            RecalculatePosition();
        }
    }

    public float RelativeDepth
    {
        get;
        set
        {
            field = value;
            RecalculateDepth();
        }
    }
    
    public Vector2 AbsolutePosition { get; private set; }
    public float AbsoluteDepth { get; private set; }

    protected abstract void DrawThis();

    public void Draw()
    {
        DrawThis();
        foreach (var child in _children)
        {
            child.Draw();
        }
    }

    public void AddChild(View child)
    {
        _children.Add(child);
        child.Parent = this;
    }

    public void RemoveChild(View child)
    {
        _children.Remove(child);
        child.Parent = null;
    }

    private void RecalculatePosition()
    {
        AbsolutePosition = Parent?.AbsolutePosition + RelativePosition ?? RelativePosition;
        foreach (var child in _children)
        {
            child.RecalculatePosition();
        }
    }

    private void RecalculateDepth()
    {
        AbsoluteDepth = Parent?.AbsoluteDepth + RelativeDepth ?? RelativeDepth;
        foreach (var child in _children)
        {
            child.RecalculateDepth();
        }
    }
}

public sealed class SpriteView : View
{
    public Sprite Sprite;

    protected override void DrawThis()
    {
        Sprite.Depth = AbsoluteDepth;
        Game.SpriteBatch.Draw(Sprite, AbsolutePosition);
    }
}

public sealed class RectangleView : View
{
    public float Width;
    public float Height;
    public Color Color;

    protected override void DrawThis()
    {
        Game.SpriteBatch.FillRectangle(
            AbsolutePosition.X - Width / 2,
            AbsolutePosition.Y - Height / 2,
            Width,
            Height,
            Color,
            AbsoluteDepth);
    }
}

public class ViewBuilder(GridLayout gridLayout)
{
    public View CreateAppearance(Entity entity)
    {
        var visible = entity.GetComponent<Visible>();
        var baseView = new RectangleView();
        if (visible is null)
            return baseView;
        baseView.Color = visible.Color;
        baseView.Width = gridLayout.CellSize.X;
        baseView.Height = gridLayout.CellSize.Y;
        return baseView;
    }
}