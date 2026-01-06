using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace PlayerSpace.UI.Common
{
    public class SearchList<T> : VisualElement
    {
        private readonly List<T> _filteredItems = new();
        private readonly List<T> _items = new();

        private readonly Func<T, string> _itemToString;
        private readonly ListView _listView;

        private readonly TextField _searchField;
        private readonly Label _searchInfo;

        public SearchList(Func<T, string> itemToString)
        {
            _itemToString = itemToString;

            _searchField = new TextField();
            _searchField.name = "searchField";
            _searchField.value = string.Empty;
            _searchField.RegisterValueChangedCallback(evt => FilterItems(evt.newValue));
            Add(_searchField);

            _searchInfo = new Label();
            _searchInfo.name = "searchInfo";
            Add(_searchInfo);

            _listView = new ListView(_filteredItems, 22, MakeItem, BindItem);
            _listView.name = "listView";
            _listView.selectionType = SelectionType.Single;
            _listView.itemsChosen += ItemChosen;
            Add(_listView);

            VisualElement MakeItem()
            {
                return new Label();
            }

            void BindItem(VisualElement e, int i)
            {
                (e as Label).text = _itemToString(_filteredItems[i]);
            }
        }

        public event Action<T> OnItemChosen;

        //TODO IList?
        public void SetItems(IEnumerable<T> items)
        {
            _items.Clear();
            _items.AddRange(items);
            FilterItems("");
        }

        //TODO Make external link to _elements safe
        private void FilterItems(string query)
        {
            _filteredItems.Clear();
            if (string.IsNullOrEmpty(query))
                _filteredItems.AddRange(_items);
            else
                _filteredItems.AddRange(
                    _items.Where(t => _itemToString(t).ToLower().Contains(query.ToLower())));
            _listView.Rebuild();
        }

        private void ItemChosen(IEnumerable<object> selection)
        {
            OnItemChosen?.Invoke((T)selection.First());
        }
    }
}