using Manhead.Core.Logic.Gameplay.View;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using R3;

namespace Manhead.Core.Logic.Editor.Data;

public class ViewHolder : IDrawable
{
    private (View? View, IDisposable? Subscription)[,] _data;
    private ViewBuilder _builder;
    private GridLayout _layout;

    public ViewHolder(ViewBuilder builder, GridLayout layout, int width, int height)
    {
        _data = new (View?, IDisposable?)[width, height];
        _builder = builder;
        _layout = layout;
    }

    public void Draw()
    {
        foreach (var (view, _) in _data)
        {
            if (view is not null)
            {
                view.Draw();
            }
        }
    }

    public void AddView(Template template, Point position)
    {
        if (_data[position.X, position.Y] is not (null, null))
        {
            Console.WriteLine($"Adding view at {position} rejected");
            return;
        }

        var view = CreateView(template, position);
        var subscription = template.Entity.Changed.Subscribe(_ => _data[position.X, position.Y].View = CreateView(template, position));
        _data[position.X, position.Y] = (view, subscription);
    }

    private View CreateView(Template template, Point position)
    {
        var view = _builder.CreateAppearance(template.Entity);
        view.RelativePosition = _layout.GridToWorld(position);
        return view;
    }

    public void RemoveView(Point position)
    {
        if (_data[position.X, position.Y] is (null, null))
        {
            Console.WriteLine($"Adding view at {position} rejected");
            return;
        }
        
        _data[position.X, position.Y].Subscription?.Dispose();
        _data[position.X, position.Y] = (null, null);
    }
}