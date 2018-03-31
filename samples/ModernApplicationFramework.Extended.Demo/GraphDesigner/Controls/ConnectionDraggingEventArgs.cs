using System.Windows;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public delegate void ConnectionDraggingEventHandler(object sender, ConnectionDraggingEventArgs e);

    public class ConnectionDraggingEventArgs : ConnectionDragEventArgs
    {
        public object Connection { get; }

        internal ConnectionDraggingEventArgs(RoutedEvent routedEvent, object source,
            ElementItem elementItem, object connection, ConnectorItem connectorItem)
            : base(routedEvent, source, elementItem, connectorItem)
        {
            Connection = connection;
        }
    }
}
