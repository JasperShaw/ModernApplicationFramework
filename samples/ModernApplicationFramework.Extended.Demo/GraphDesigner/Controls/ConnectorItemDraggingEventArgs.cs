using System.Windows;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    internal delegate void ConnectorItemDraggingEventHandler(object sender, ConnectorItemDraggingEventArgs e);

    public class ConnectorItemDraggingEventArgs : RoutedEventArgs
    {
        public double VerticalChange { get; }

        public double HorizontalChange { get; }

        public ConnectorItemDraggingEventArgs(RoutedEvent routedEvent, object source, double horizontalChange, double verticalChange) :
            base(routedEvent, source)
        {
            HorizontalChange = horizontalChange;
            VerticalChange = verticalChange;
        }
    }
}
