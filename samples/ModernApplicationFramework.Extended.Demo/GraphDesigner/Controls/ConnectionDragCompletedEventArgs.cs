using System.Windows;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public delegate void ConnectionDragCompletedEventHandler(object sender, ConnectionDragCompletedEventArgs e);

    public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
    {
        public object Connection { get; }
        internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source,
            ElementItem elementItem, object connection, ConnectorItem sourceConnectorItem)
            : base(routedEvent, source, elementItem, sourceConnectorItem)
        {
            Connection = connection;
        }
    }
}
