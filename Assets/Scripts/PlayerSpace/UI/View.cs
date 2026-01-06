using System;
using R3;
using UnityEngine.UIElements;
using VContainer.Unity;

namespace PlayerSpace.UI
{
    public abstract class View<TViewModel> : IInitializable, IDisposable
        where TViewModel : ViewModel
    {
        protected readonly CompositeDisposable Disposables = new();
        protected TViewModel ViewModel;

        public View(TViewModel viewModel, VisualElement root, bool hideOnAwake = false)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            Root = root ?? throw new ArgumentNullException(nameof(root));
            if (hideOnAwake) Hide();
        }

        public VisualElement Root { get; }
        public bool IsVisible => Root.style.display == DisplayStyle.Flex;

        public virtual void Dispose()
        {
            Disposables.Dispose();
            //TODO: there were variant Root?.parent.Remove(Root); 
            Root.Clear();
            ViewModel.Dispose();
        }

        public void Initialize()
        {
            SetVisualElements();
            BindViewData();
            RegisterInputCallbacks();
        }

        protected virtual void SetVisualElements()
        {
        }

        protected virtual void BindViewData()
        {
        }

        protected virtual void RegisterInputCallbacks()
        {
        }

        public void Show()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
    }
}