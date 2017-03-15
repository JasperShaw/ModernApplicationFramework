using System.Windows;
using ModernApplicationFramework.Core.Platform.Enums;

namespace ModernApplicationFramework.Core.Events
{
    public class PreviewTextChangedEventArgs : RoutedEventArgs
    {

        public PreviewTextChangedEventArgs(RoutedEvent routedEvent, TextChangedType type, string text)
            : base(routedEvent)
        {
            Type = type;
            Text = text;
        }

        public PreviewTextChangedEventArgs(RoutedEvent routedEvent, object source, TextChangedType type, string text)
            : base(routedEvent, source)
        {
            Type = type;
            Text = text;
        }

        public string Text { get; }

        public TextChangedType Type { get; }

    }
}