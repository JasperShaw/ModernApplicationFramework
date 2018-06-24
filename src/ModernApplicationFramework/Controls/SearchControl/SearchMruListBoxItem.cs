using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Interfaces.Controls.Search;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchMruListBoxItem : ListBoxItem, ISearchControlPopupLocation
    {
        public static readonly RoutedEvent MruItemSelectedEvent = EventManager.RegisterRoutedEvent(nameof(MruItemSelectedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchMruListBoxItem));
        public static readonly RoutedEvent MruItemDeletedEvent = EventManager.RegisterRoutedEvent(nameof(MruItemDeletedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchMruListBoxItem));

        public event RoutedEventHandler MruItemSelected
        {
            add => AddHandler(MruItemSelectedEvent, value);
            remove => RemoveHandler(MruItemSelectedEvent, value);
        }

        public event RoutedEventHandler MruItemDeleted
        {
            add => AddHandler(MruItemDeletedEvent, value);
            remove => RemoveHandler(MruItemDeletedEvent, value);
        }

        void ISearchControlPopupLocation.OnKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Return || e.Key == Key.Space)
            {
                RaiseEvent(new RoutedEventArgs(MruItemSelectedEvent));
                e.Handled = true;
            }
            else
            {
                if (e.Key != Key.Delete)
                    return;
                RaiseEvent(new RoutedEventArgs(MruItemDeletedEvent));
                e.Handled = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            RaiseEvent(new RoutedEventArgs(MruItemSelectedEvent));
            e.Handled = true;
        }
    }
}
