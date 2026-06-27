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
        ManheadGame.SpriteBatch.Draw(Sprite, position + AbsolutePosition);
    }
}

public sealed class RectangleView : View
{
    public float Width;
    public float Height;
    public Color Color;

    protected override void DrawThis(Vector2 position)
    {
        ManheadGame.SpriteBatch.DrawRectangle(
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
        var baseView = new RectangleView();
        var cellSize = gridLayout.CellSize;

        if (entity.GetComponent<Visible>() is { } visible)
        {
            baseView.Color = visible.Color;
            baseView.Width = cellSize.X;
            baseView.Height = cellSize.Y;
        }

        var border = 5;
        
        if (entity.LeftSide.GetComponent<Wall>() is { } leftWall)
        {
            var view = new RectangleView();
            view.Color = leftWall.Color;
            view.RelativePosition = new (-(cellSize.X / 2), 0);
            view.Width = border;
            view.Height = cellSize.Y;
            view.RelativeDepth = 0.1f;
            baseView.AddChild(view);
        }
        
        if (entity.RightSide.GetComponent<Wall>() is { } rightWall)
        {
            var view = new RectangleView();
            view.Color = rightWall.Color;
            view.RelativePosition = new (cellSize.X / 2, 0);
            view.Width = border;
            view.Height = cellSize.Y;
            view.RelativeDepth = 0.1f;
            baseView.AddChild(view);
        }
        
        if (entity.FrontSide.GetComponent<Wall>() is { } frontSide)
        {
            var view = new RectangleView();
            view.Color = frontSide.Color;
            view.RelativePosition = new (0, - (cellSize.Y / 2));
            view.Width = cellSize.X;
            view.Height = border;
            view.RelativeDepth = 0.1f;
            baseView.AddChild(view);
        }
        
        if (entity.BackSide.GetComponent<Wall>() is { } backSide)
        {
            var view = new RectangleView();
            view.Color = backSide.Color;
            view.RelativePosition = new (0, cellSize.Y / 2);
            view.Width = cellSize.X;
            view.Height = border;
            view.RelativeDepth = 0.1f;
            baseView.AddChild(view);
        }

        if (entity.GetComponent<Player>() is { } player)
        {
            var leftEye = new RectangleView();
            leftEye.Color = Color.Black;
            leftEye.Width = cellSize.X / 5;
            leftEye.Height = cellSize.Y / 5;
            leftEye.RelativePosition = new (- cellSize.X / 4, - cellSize.Y / 5);
            var rightEye = new RectangleView();
            rightEye.Color = Color.Black;
            rightEye.Width = cellSize.X / 5;
            rightEye.Height = cellSize.Y / 5;
            rightEye.RelativePosition = new (cellSize.X / 4, - cellSize.Y / 5);
            
            var view = new ViewContainer();
            view.RelativeDepth = 0.2f;
            view.AddChild(leftEye);
            view.AddChild(rightEye);
            baseView.AddChild(view);
        }
        
        return baseView;
    }
}