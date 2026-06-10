using Gum.Forms.Controls;
using ModelMediator.Abstractions;
using R3;

namespace Manhead.Core.Logic.Editor.UI.Common;

public class IntField<TProperty> : TextBox, IDisposable
    where TProperty : struct, IProperty<int>
{
    private readonly IDisposable _subscription;
    
    public IntField(TProperty property)
    {
        Name = property.Name;
        PreviewTextInput += (_, args) =>
        {
            if (int.TryParse(Text.Insert(CaretIndex, args.Text), out var value))
            {
                property.TrySet(value);
            }
            
            args.Handled = true;
        };

        _subscription = property.Changed.Subscribe(next =>
        {
            Text = next.ToString();
            CaretIndex++;
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}