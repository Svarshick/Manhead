using Manhead.Core.Logic.Gameplay.View;
using Manhead.Core.Logic.WorldSpace;

namespace Manhead.Core.Logic.Gameplay.Data;

public class EntityView
{
    private Dictionary<Entity, View.View> _views = new();
    private readonly ViewBuilder _builder;

    public EntityView(GridLayout gridLayout)
    {
        _builder = new ViewBuilder(gridLayout);
    }

    public View.View AddView(Entity entity)
    {
        if (_views.ContainsKey(entity))
            throw new Exception();

        var view = new ViewContainer();
        view.AddChild(_builder.Build(entity));
        _views.Add(entity, view);
        return view;
    }

    public void RefreshView(Entity entity)
    {
        var view = _builder.Build(entity);
        _views[entity].ClearChild();
        _views[entity].AddChild(view);
    }

    public View.View GetView(Entity entity)
    {
        return _views[entity];
    }

    public void Draw()
    {
        foreach (var view in _views.Values)
        {
            view.Draw();
        }
    }
}