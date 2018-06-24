using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public class GraphControl : Control
    {
        public event SelectionChangedEventHandler SelectionChanged;

        private ElementItemsControl _elementItemsControl;
        private ConnectorItem _draggingSourceConnector;
        private object _draggingConnectionDataContext;

        public static readonly RoutedEvent ConnectionDragStartedEvent = EventManager.RegisterRoutedEvent(
            "ConnectionDragStarted", RoutingStrategy.Bubble, typeof(ConnectionDragStartedEventHandler),
            typeof(GraphControl));

        public static readonly RoutedEvent ConnectionDraggingEvent = EventManager.RegisterRoutedEvent(
            "ConnectionDragging", RoutingStrategy.Bubble, typeof(ConnectionDraggingEventHandler),
            typeof(GraphControl));

        public static readonly RoutedEvent ConnectionDragCompletedEvent = EventManager.RegisterRoutedEvent(
            "ConnectionDragCompleted", RoutingStrategy.Bubble, typeof(ConnectionDragCompletedEventHandler),
            typeof(GraphControl));


        public static readonly DependencyProperty ElementsSourceProperty = DependencyProperty.Register(
           "ElementsSource", typeof(IEnumerable), typeof(GraphControl));

        public static readonly DependencyProperty ElementItemContainerStyleProperty = DependencyProperty.Register(
            "ElementItemContainerStyle", typeof(Style), typeof(GraphControl));

        public static readonly DependencyProperty ElementItemTemplateProperty = DependencyProperty.Register(
            "ElementItemTemplate", typeof(DataTemplate), typeof(GraphControl));

        public static readonly DependencyProperty ElementItemDataTemplateSelectorProperty = DependencyProperty.Register(
            "ElementItemDataTemplateSelector", typeof(DataTemplateSelector), typeof(GraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ConnectionItemDataTemplateSelectorProperty = DependencyProperty.Register(
            "ConnectionItemDataTemplateSelector", typeof(DataTemplateSelector), typeof(GraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ConnectionsSourceProperty = DependencyProperty.Register(
            "ConnectionsSource", typeof(IEnumerable), typeof(GraphControl));

        public static readonly DependencyProperty ConnectionItemContainerStyleProperty = DependencyProperty.Register(
            "ConnectionItemContainerStyle", typeof(Style), typeof(GraphControl));

        public static readonly DependencyProperty ConnectionItemTemplateProperty = DependencyProperty.Register(
            "ConnectionItemTemplate", typeof(DataTemplate), typeof(GraphControl));

        public static readonly DependencyProperty MultiselectEnabledProperty = DependencyProperty.Register(
            "MultiselectEnabled", typeof(bool), typeof(GraphControl), new PropertyMetadata(default(bool)));

        public event ConnectionDragStartedEventHandler ConnectionDragStarted
        {
            add => AddHandler(ConnectionDragStartedEvent, value);
            remove => RemoveHandler(ConnectionDragStartedEvent, value);
        }

        public event ConnectionDraggingEventHandler ConnectionDragging
        {
            add => AddHandler(ConnectionDraggingEvent, value);
            remove => RemoveHandler(ConnectionDraggingEvent, value);
        }

        public event ConnectionDragCompletedEventHandler ConnectionDragCompleted
        {
            add => AddHandler(ConnectionDragCompletedEvent, value);
            remove => RemoveHandler(ConnectionDragCompletedEvent, value);
        }

        public IEnumerable ElementsSource
        {
            get => (IEnumerable)GetValue(ElementsSourceProperty);
            set => SetValue(ElementsSourceProperty, value);
        }

        public Style ElementItemContainerStyle
        {
            get => (Style)GetValue(ElementItemContainerStyleProperty);
            set => SetValue(ElementItemContainerStyleProperty, value);
        }

        public DataTemplate ElementItemTemplate
        {
            get => (DataTemplate)GetValue(ElementItemTemplateProperty);
            set => SetValue(ElementItemTemplateProperty, value);
        }

        public DataTemplateSelector ElementItemDataTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ElementItemDataTemplateSelectorProperty);
            set => SetValue(ElementItemDataTemplateSelectorProperty, value);
        }

        public DataTemplateSelector ConnectionItemDataTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ConnectionItemDataTemplateSelectorProperty);
            set => SetValue(ConnectionItemDataTemplateSelectorProperty, value);
        }

        public IEnumerable ConnectionsSource
        {
            get => (IEnumerable)GetValue(ConnectionsSourceProperty);
            set => SetValue(ConnectionsSourceProperty, value);
        }

        public Style ConnectionItemContainerStyle
        {
            get => (Style)GetValue(ConnectionItemContainerStyleProperty);
            set => SetValue(ConnectionItemContainerStyleProperty, value);
        }

        public DataTemplate ConnectionItemTemplate
        {
            get => (DataTemplate)GetValue(ConnectionItemTemplateProperty);
            set => SetValue(ConnectionItemTemplateProperty, value);
        }

        public bool MultiselectEnabled
        {
            get => (bool)GetValue(MultiselectEnabledProperty);
            set => SetValue(MultiselectEnabledProperty, value);
        }

        public IList SelectedElements => _elementItemsControl.SelectedItems;

        internal IEnumerable<ElementItem> SelectedElementItems
        {
            get
            {
                var list = new List<ElementItem>();
                foreach (var selectedElement in SelectedElements)
                {
                    if (!(_elementItemsControl.ItemContainerGenerator.ContainerFromItem(selectedElement) is ElementItem elementItem))
                        continue;
                    list.Add(elementItem);
                }
                return list;
            }
        }

        static GraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl),
                new FrameworkPropertyMetadata(typeof(GraphControl)));
        }

        public GraphControl()
        {
            AddHandler(ConnectorItem.ConnectorDragStartedEvent, new ConnectorItemDragStartedEventHandler(OnConnectorItemDragStarted));
            AddHandler(ConnectorItem.ConnectorDraggingEvent, new ConnectorItemDraggingEventHandler(OnConnectorItemDragging));
            AddHandler(ConnectorItem.ConnectorDragCompletedEvent, new ConnectorItemDragCompletedEventHandler(OnConnectorItemDragCompleted));
        }

        public override void OnApplyTemplate()
        {
            _elementItemsControl = (ElementItemsControl)Template.FindName("PART_ElementItemsControl", this);
            _elementItemsControl.SelectionChanged += OnElementItemsControlSelectChanged;
            base.OnApplyTemplate();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement frameworkElement)
            {
                var elementItem = frameworkElement.FindAncestorOrSelf<ListBoxItem>();
                if (elementItem != null)
                    return;
                _elementItemsControl.SelectedItems.Clear();
            }
            base.OnMouseLeftButtonDown(e);
        }

        private Point _lastMousePosition;
        private bool _isLeftMouseButtonDown;
        private bool _isDragging;

        internal bool IsDragging => _isDragging;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var elementItem = e.GetOriginalSource<ListBoxItem>();
            if (elementItem != null)
            {
                _lastMousePosition = e.GetPosition(this);
                _isLeftMouseButtonDown = true;
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var t = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (t == null)
                return;
            var el = t.VisualHit as FrameworkElement;
            var elm = el?.FindAncestor<ElementItem>();
            if (elm == null)
                return;

            if (_isDragging)
            {
                var newMousePosition = e.GetPosition(this);
                var delta = newMousePosition - _lastMousePosition;

                SelectedElementItems.ForEach(x =>
                {
                    x.X += delta.X;
                    x.Y += delta.Y;
                });
                _lastMousePosition = newMousePosition;
            }

            if (_isLeftMouseButtonDown)
            {
                _isDragging = true;
                elm.CaptureMouse();
            }
            base.OnMouseMove(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isLeftMouseButtonDown)
            {
                _isLeftMouseButtonDown = false;
                if (_isDragging)
                {
                    WasDragged = true;
                    _isDragging = false;
                    e.GetOriginalSource<ListBoxItem>()?.ReleaseMouseCapture();
                }
            }
            base.OnPreviewMouseLeftButtonUp(e);
            
        }

        public bool WasDragged { get; protected set; }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            WasDragged = false;
        }

        internal int GetMaxZIndex()
        {
            return _elementItemsControl.Items.Cast<object>()
                .Select(item => (ElementItem)_elementItemsControl.ItemContainerGenerator.ContainerFromItem(item))
                .Select(elementItem => elementItem.ZIndex)
                .Concat(new[] { 0 })
                .Max();
        }

        private void OnElementItemsControlSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            var handler = SelectionChanged;
            handler?.Invoke(this, new SelectionChangedEventArgs(Selector.SelectionChangedEvent, e.RemovedItems, e.AddedItems));
        }

        private void OnConnectorItemDragStarted(object sender, ConnectorItemDragStartedEventArgs e)
        {
            e.Handled = true;

            _draggingSourceConnector = (ConnectorItem)e.OriginalSource;

            var eventArgs = new ConnectionDragStartedEventArgs(ConnectionDragStartedEvent, this,
                _draggingSourceConnector.ParentElementItem, _draggingSourceConnector);
            RaiseEvent(eventArgs);

            _draggingConnectionDataContext = eventArgs.Connection;

            if (_draggingConnectionDataContext == null)
                e.Cancel = true;
        }

        private void OnConnectorItemDragging(object sender, ConnectorItemDraggingEventArgs e)
        {
            e.Handled = true;

            var connectionDraggingEventArgs =
                new ConnectionDraggingEventArgs(ConnectionDraggingEvent, this,
                    _draggingSourceConnector.ParentElementItem, _draggingConnectionDataContext,
                    _draggingSourceConnector);
            RaiseEvent(connectionDraggingEventArgs);
        }

        private void OnConnectorItemDragCompleted(object sender, ConnectorItemDragCompletedEventArgs e)
        {
            e.Handled = true;

            RaiseEvent(new ConnectionDragCompletedEventArgs(ConnectionDragCompletedEvent, this,
                _draggingSourceConnector.ParentElementItem, _draggingConnectionDataContext,
                _draggingSourceConnector));

            _draggingSourceConnector = null;
            _draggingConnectionDataContext = null;
        }
    }
}
