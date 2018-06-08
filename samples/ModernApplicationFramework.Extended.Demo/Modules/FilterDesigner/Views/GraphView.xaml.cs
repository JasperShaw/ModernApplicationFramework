using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.Views
{
    public partial class GraphView
    {
        private Point _originalContentMouseDownPoint;

        private GraphViewModel ViewModel => (GraphViewModel) DataContext;

        public GraphView()
        {
            InitializeComponent();
            CommandHelpers.RegisterCommandHandler(GetType(), EditingCommands.Delete, OnDelete, CanDelete);
        }

        protected virtual bool CanDelete()
        {
            return DataContext is GraphViewModel;
        }

        protected virtual void OnDelete()
        {
            ((GraphViewModel) DataContext).DeleteSelectedElements();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            Focus();
            Keyboard.Focus(this);
            base.OnPreviewMouseDown(e);
        }

        private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(sender is GraphView graphView))
                e.CanExecute = false;
            else
                e.CanExecute = graphView.CanDelete();
        }

        private void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {
            ((GraphView) sender)?.OnDelete();
        }

        private void OnGraphControlConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var currentDragPoint = Mouse.GetPosition(GraphControl);
            var sourceConnector = (ConnectorViewModel) e.SourceConnector.DataContext;
            var newConnection = (ConnectionViewModel) e.Connection;
            ViewModel.OnConnectionDragCompleted(currentDragPoint, newConnection, sourceConnector);
        }

        private void OnGraphControlConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            var currentDragPoint = Mouse.GetPosition(GraphControl);
            var connection = (ConnectionViewModel) e.Connection;
            ViewModel.OnConnectionDragging(currentDragPoint, connection);
        }

        private void OnGraphControlConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            var sourceConnector = (ConnectorViewModel) e.SourceConnector.DataContext;
            var currentDragPoint = Mouse.GetPosition(GraphControl);
            var connection = ViewModel.OnConnectionDragStarted(sourceConnector, currentDragPoint);
            e.Connection = connection;
        }

        private void OnGraphControlMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && GraphControl.IsMouseCaptured)
            {
                var currentContentMousePoint = e.GetPosition(GraphControl);
                var dragOffset = currentContentMousePoint - _originalContentMouseDownPoint;

                ZoomAndPanControl.ContentOffsetX -= dragOffset.X;
                ZoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
        }

        private void OnGraphControlMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ZoomAndPanControl.ZoomAboutPoint(
                ZoomAndPanControl.ContentScale + e.Delta / 1000.0f,
                e.GetPosition(GraphControl));

            e.Handled = true;
        }

        private void OnGraphControlRightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            _originalContentMouseDownPoint = e.GetPosition(GraphControl);
            GraphControl.CaptureMouse();
            Mouse.OverrideCursor = Cursors.ScrollAll;
            e.Handled = true;
        }

        private void OnGraphControlRightMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = null;
            GraphControl.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void OnGraphControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.OnSelectionChanged();
        }
    }
}