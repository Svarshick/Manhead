using ModelMediator.Abstractions;

namespace Manhead.Core.Logic.Editor.UI.Common;

public class DefaultSystem : ISystem
{
    public bool SetProperty<TValue, TProperty>(TProperty property, TValue value)
        where TProperty : struct, IDirectProperty<TValue>
    {
        property.Value = value;
        return true;
    }

    public bool AddItem<TValue, TListProperty>(TListProperty property, TValue item)
        where TListProperty : struct, IDirectListProperty<TValue>
    {
        property.Add(item);
        return true;
    }

    public bool RemoveItem<TValue, TListProperty>(TListProperty property, TValue item)
        where TListProperty : struct, IDirectListProperty<TValue>
    {
        property.Remove(item);
        return true;
    }

    public bool RemoveItemAt<TValue, TListProperty>(TListProperty property, int index)
        where TListProperty : struct, IDirectListProperty<TValue>
    {
        property.RemoveAt(index);
        return true;
    }

    public bool SetItem<TValue, TListProperty>(TListProperty property, int index, TValue item)
        where TListProperty : struct, IDirectListProperty<TValue>
    {
        property.Set(index, item);
        return true;
    }

    public bool ClearList<TValue, TListProperty>(TListProperty property)
        where TListProperty : struct, IDirectListProperty<TValue>
    {
        property.Clear();
        return true;
    }
}