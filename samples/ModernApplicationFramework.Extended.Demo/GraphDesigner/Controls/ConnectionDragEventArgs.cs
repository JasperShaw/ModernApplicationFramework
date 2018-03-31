using System.Windows;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public abstract class ConnectionDragEventArgs : RoutedEventArgs
    {
        public ElementItem ElementItem { get; }

        public ConnectorItem SourceConnector { get; }

        protected ConnectionDragEventArgs(RoutedEvent routedEvent, object source,
            ElementItem elementItem, ConnectorItem sourceConnectorItem)
            : base(routedEvent, source)
        {
            ElementItem = elementItem;
            SourceConnector = sourceConnectorItem;
        }
    }
}
