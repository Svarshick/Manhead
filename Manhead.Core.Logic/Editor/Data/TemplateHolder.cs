using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.View;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using ObservableCollections;

namespace Manhead.Core.Logic.Editor.Data;

public class TemplateHolder
{
    public readonly ObservableDictionary<Template, List<Placement>> Templates = new();
    private readonly Field<Placement> _field;
    public readonly TemplateViewHolder TemplatesView;

    public TemplateHolder(GridLayout gridLayout, int width, int height)
    {
        var viewBuilder = new ViewBuilder(gridLayout);
        var viewHolder = new TemplateViewHolder(viewBuilder, gridLayout, width, height);
        TemplatesView = viewHolder;
        _field = new(width, height);
    }

    public void AddTemplate(Template template)
    {
        TemplatesView.AddTemplate(template);
        Templates.Add(template, new List<Placement>());
    }

    public void RemoveTemplate(Template template)
    {
        foreach (var placement in Templates[template])
        {
            TemplatesView.RemovePlacement(placement.Position);
        }
        TemplatesView.RemoveTemplate(template);
        Templates.Remove(template);
    }

    public void AddPlacement(Template template, Point position)
    {
        foreach (var placement in Templates[template])
        {
            if (placement.Position == position)
            {
                Console.WriteLine($"Adding placement at {position} rejected");
                return;
            }
        }
        Templates[template].Add(new Placement(template, position));
        _field[position].Add(new Placement(template, position));
        TemplatesView.AddPlacement(template, position);
    }

    public void RemovePlacement(Point position)
    {
        var template = _field[position].First().Template;
        _field[position].Clear();
        Templates[template].RemoveAll(placement => placement.Position == position);
        TemplatesView.RemovePlacement(position);
    }

    public Placement GetPlacement(Point position) => _field[position].First();
    
    public bool IsFree(Point position)
    {
        return _field.InBounds(position) && _field[position].Count <= 0;
    }

    public bool IsFilled(Point position)
    {
        return _field.InBounds(position) && _field[position].Count > 0;
    }
}