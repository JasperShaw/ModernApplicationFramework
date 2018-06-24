using System.Windows;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchOptionButton
    {
        public static readonly RoutedEvent OptionButtonClickedEvent = EventManager.RegisterRoutedEvent("OptionButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchOptionButton));

        public static void AddOptionButtonClickedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement)?.AddHandler(OptionButtonClickedEvent, handler);
        }

        public static void RemoveOptionButtonClickedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement)?.RemoveHandler(OptionButtonClickedEvent, handler);
        }
    }
}
