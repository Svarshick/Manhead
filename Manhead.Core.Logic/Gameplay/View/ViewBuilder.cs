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

    protected abstract void DrawThis(Vector2 position);

    public void Draw()
    {
        DrawThis(Vector2.Zero);
        foreach (var child in _children)
        {
            child.Draw();
        }
    }

    public void DrawAt(Vector2 position)
    {
        DrawThis(position);
        foreach (var child in _children)
        {
            child.DrawAt(position);
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

    public void ClearChild()
    {
        foreach (var child in _children)
        {
            child.Parent = null;
        }
        _children.Clear();
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

    protected override void DrawThis(Vector2 position)
    {
        Sprite.Depth = AbsoluteDepth;
        Game.SpriteBatch.Draw(Sprite, position + AbsolutePosition);
    }
}

public sealed class RectangleView : View
{
    public float Width;
    public float Height;
    public Color Color;

    protected override void DrawThis(Vector2 position)
    {
        Game.SpriteBatch.DrawRectangle(
            position.X + AbsolutePosition.X - Width / 2,
            position.Y + AbsolutePosition.Y - Height / 2,
            Width,
            Height,
            Color,
            Math.Min(Width, Height),
            AbsoluteDepth);
    }
}

public sealed class ViewContainer : View
{
    protected override void DrawThis(Vector2 position)
    {
    }
}

public class ViewBuilder(GridLayout gridLayout)
{
    public View Build(Entity entity)
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