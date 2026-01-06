using System.Collections.Generic;
using Data;
using UnityEngine.UIElements;

namespace PlayerSpace.UI.MainMenu
{
    public class MainMenuView : View<MainMenuViewModel>
    {
        private readonly Dictionary<Button, GameSave> _saveButtons = new();

        public MainMenuView(MainMenuViewModel viewModel, VisualElement root) : base(viewModel, root)
        {
        }

        protected override void SetVisualElements()
        {
            foreach (var save in ViewModel.Cells)
            {
                var button = new Button();
                _saveButtons[button] = save;
                Root.Add(button);
            }
        }

        protected override void BindViewData()
        {
            foreach (var (button, save) in _saveButtons)
                if (save is null)
                    button.text = "Empty save";
                else
                    button.text = save.Directory.Directory.Name;
        }

        protected override void RegisterInputCallbacks()
        {
            foreach (var (button, save) in _saveButtons)
                if (save is not null)
                    button.RegisterCallback<ClickEvent>(e => ViewModel.LoadGame(save));
        }
    }
}