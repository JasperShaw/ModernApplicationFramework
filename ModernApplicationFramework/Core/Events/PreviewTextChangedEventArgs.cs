using System.Windows;
using ModernApplicationFramework.Core.Input;

namespace ModernApplicationFramework.Core.Events
{
    public class PreviewTextChangedEventArgs : RoutedEventArgs
    {
        #region Constructors

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

        #endregion

        #region Properties

        public string Text { get; private set; }

        public TextChangedType Type { get; private set; }

        #endregion
    }
}
