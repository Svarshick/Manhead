using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Editor.Data;

public class TemplatesHolder
{
    private readonly Dictionary<Template, List<Placement>> _data = new();
    public readonly ViewHolder View;

    public TemplatesHolder(ViewHolder viewHolder)
    {
        View = viewHolder;
    }

    public void AddTemplate(Template template)
    {
        _data.Add(template, new List<Placement>());
    }

    public void RemoveTemplate(Template template)
    {
        foreach (var placement in _data[template])
        {
            View.RemoveView(placement.Position);
        }
        _data.Remove(template);
    }

    public void AddPlacement(Template template, Point position)
    {
        foreach (var placement in _data[template])
        {
            if (placement.Position == position)
            {
                Console.WriteLine($"Adding placement at {position} rejected");
                return;
            }
        }
        _data[template].Add(new Placement(template, position));
        View.AddView(template, position);
    }

    public void RemovePlacement(Template template, Point position)
    {
        var placements = _data[template];
        for (int i = 0; i < placements.Count; i++)
        {
            if (placements[i].Position == position)
            {
                placements.RemoveAt(i);
                View.RemoveView(position);
                return;
            }
        }
    }
}