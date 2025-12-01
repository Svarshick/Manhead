using System;
using UnityEngine.UIElements;

namespace PlayerSpace.UI
{
    public sealed class SafeUIDocument
    {
        public VisualElement Root => LazyRoot.Value;
        public Lazy<VisualElement> LazyRoot { get; }

        public SafeUIDocument(UIDocument uiDocument)
        {
            LazyRoot = new Lazy<VisualElement>(() =>
            {
                if (uiDocument.rootVisualElement != null)
                    return uiDocument.rootVisualElement;
                if (uiDocument.visualTreeAsset == null)
                    throw new ArgumentNullException();
                ForceRecreationOfRoot();
                if (uiDocument.rootVisualElement == null)
                    throw new InvalidOperationException();
                return uiDocument.rootVisualElement;
            });
            return;
            void ForceRecreationOfRoot() => uiDocument.visualTreeAsset = uiDocument.visualTreeAsset;
        }
    }

}