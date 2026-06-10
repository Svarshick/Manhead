using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.View;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using ObservableCollections;
using R3;

namespace Manhead.Core.Logic.Editor.Data;

public class TemplateViewHolder : IDrawable
{
    private Dictionary<Template, (View View, IDisposable Subscription)> _templates;
    private View?[,] _placements;
    private ViewBuilder _builder;
    private GridLayout _layout;

    public TemplateViewHolder(ViewBuilder builder, GridLayout layout, int width, int height)
    {
        _templates = new();
        _placements = new View?[width, height];
        _builder = builder;
        _layout = layout;
    }

    public void Draw()
    {
        for (int x = 0; x < _placements.GetLength(0); x++)
        {
            for (int y = 0; y < _placements.GetLength(1); y++)
            {
                var view = _placements[x, y];
                if (view is null)
                    continue;
                
                var position = _layout.GridToWorld(new Point(x, y));
                view.DrawAt(position);
            }
        }
    }

    public void AddTemplate(Template template)
    {
        var view = new ViewContainer();
        view.AddChild(_builder.Build(template.Entity));
        var disposables = new CompositeDisposable();
        ObserveHolder(template.Entity, disposables, template);
        ObserveHolder(template.Entity.LeftSide, disposables, template);
        ObserveHolder(template.Entity.RightSide, disposables, template);
        ObserveHolder(template.Entity.FrontSide, disposables, template);
        ObserveHolder(template.Entity.BackSide, disposables, template);
        _templates.Add(template, (view, disposables));
    }

    private void ObserveHolder(IComponentHolder holder, CompositeDisposable disposables, Template template)
    {
        var componentAdd = holder.Components
            .ObserveAdd()
            .Subscribe(evt =>
            {
                RefreshView(template);
                disposables.Add(evt.Value.Changed.Subscribe(_ => RefreshView(template)));
            });
        disposables.Add(componentAdd);
        var componentRm = holder.Components
            .ObserveRemove()
            .Subscribe(evg =>
            {
                RefreshView(template);
            });
        disposables.Add(componentRm);
    }

    private void RefreshView(Template template)
    {
        var value = _templates[template];
        value.View.ClearChild(); 
        value.View.AddChild(_builder.Build(template.Entity));
        _templates[template] = value;
    }

    //removes template ONLY 
    public void RemoveTemplate(Template template)
    {
        var value = _templates[template];
        value.Subscription?.Dispose();
        _templates.Remove(template);
    }
    
    public void AddPlacement(Template template, Point position)
    {
        if (_placements[position.X, position.Y] is not null)
        {
            Console.WriteLine($"Adding view at {position} rejected");
            return;
        }

        var view = _templates[template].View;
        _placements[position.X, position.Y] = view;
    }

    public void RemovePlacement(Point position)
    {
        if (_placements[position.X, position.Y] is null)
        {
            Console.WriteLine($"Adding view at {position} rejected");
            return;
        }
        
        _placements[position.X, position.Y] = null;
    }
}