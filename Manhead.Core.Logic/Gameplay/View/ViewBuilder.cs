using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;

namespace Manhead.Core.Logic.Gameplay.View;

public sealed class View : IDrawable
{
    public Vector2 Position;
    public Sprite Sprite;
    public View Parent;

    public Vector2 WorldPosition => Parent?.WorldPosition + Position ?? Position;
    
    public void Draw()
    {
    }
}

public class ViewBuilder
{
    public View CreateAppearance(Entity entity)
    {
        var visible = entity.Components.OfType<Visible>().FirstOrDefault();
        if (visible is null)
            return new View();
        return null;
    }
}