using UnityEngine.UIElements;

namespace PlayerSpace.UI
{
    public static class UIExtensions
    {
        public static void SetVisibility(this VisualElement element, bool toShow)
        {
            element.style.visibility = toShow ? Visibility.Visible : Visibility.Hidden;
        }

        public static bool IsVisible(this VisualElement element)
        {
            return element.style.visibility == Visibility.Visible;
        }

        public static void SetDisplay(this VisualElement element, bool toDisplay)
        {
            element.style.display = toDisplay ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static bool IsDisplayed(this VisualElement element)
        {
            return element.style.display == DisplayStyle.Flex;
        }
    }
}