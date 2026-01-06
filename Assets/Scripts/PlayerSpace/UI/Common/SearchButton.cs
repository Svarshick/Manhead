using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PlayerSpace.UI.Common
{
    public class SearchButton<T> : VisualElement
    {
        private readonly Button _searchButton;
        private readonly SearchList<T> _searchList;

        public SearchButton(string text, Func<T, string> itemToString)
        {
            _searchButton = new Button(() => TogglePopup()) { text = text };
            _searchButton.name = "searchButton";
            Add(_searchButton);

            _searchList = new SearchList<T>(itemToString);
            _searchList.name = "searchList";
            _searchList.SetDisplay(false);
            _searchList.OnItemChosen += ItemChosen;
            RegisterCallback<PointerLeaveEvent>(evt => _searchList.SetDisplay(false));
            Add(_searchList);
        }

        public event Action<T> OnItemChosen;

        private void TogglePopup()
        {
            _searchList.SetDisplay(!_searchList.IsDisplayed());
            if (_searchList.IsDisplayed()) _searchList.Focus();
        }

        private void ItemChosen(T item)
        {
            _searchList.SetDisplay(false);
            OnItemChosen?.Invoke(item);
        }

        public void SetItems(IEnumerable<T> items)
        {
            _searchList.SetItems(items);
        }
    }
}