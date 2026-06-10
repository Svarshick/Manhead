using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;

namespace Manhead.Core.Logic.Editor.UI.Common;

public class SearchDialog : ContainerRuntime
{
    private readonly List<(string Text, object Context)> _options;

    public event Action<string, object>? Choose;
        
    public readonly TextBox SearchBox;
    public readonly ListBox ListBox;

    public SearchDialog(IReadOnlyList<(string Text, object Context)> options)
    {
        Name = "SearchDialog";
        _options = new(options);
        _options.Sort((a, b) => string.Compare(a.Text, b.Text, StringComparison.Ordinal));

        var grid = new Grid
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
            RowDefinitions =
            {
                new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                new RowDefinition(new GridLength(1, GridUnitType.Star)),
            }
        };
        this.AddChild(grid);

        SearchBox = new TextBox
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            Width = 0
        };
        grid.AddChild(SearchBox, 0, 0);

        ListBox = new ListBox
        {
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Width = 0,
            Height = 0,
        };
        grid.AddChild(ListBox, 1, 0);

        SearchBox.TextChanged += (sender, args) => FilterOptions();
    }

    public void FilterOptions()
    {
        var filter = SearchBox.Text;
        ListBox.Items.Clear();
        foreach (var (text, option) in _options)
        {
            if (string.IsNullOrEmpty(filter) ||
                text.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                var view = new Button
                {
                    WidthUnits = DimensionUnitType.RelativeToParent,
                    Width = 0,
                    Text = text,
                };
                view.Click += (_, _) => Choose?.Invoke(text, option);
                ListBox.Items.Add(view);
            }
        }
    }
}