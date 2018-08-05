using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class TextViewHost : ContentControl, ITextViewHost
    {
        private bool _isClosed;
        private readonly bool _setFocus;
        private readonly TextEditorFactoryService _factory;
        private bool _hasInitializeBeenCalled;
        private Grid _outerGrid;
        private readonly ITextView _textView;
        private static ResourceDictionary _editorResources;
        private readonly IList<ITextViewMargin> _edges = new List<ITextViewMargin>(5);
        private readonly MouseWheelHelper _wheelHelper = new MouseWheelHelper();

        public event EventHandler Closed;

        public bool IsClosed => _isClosed;

        public Control HostControl => this;

        public ITextView TextView => _textView;

        public bool IsReadOnly { get; set; }

        internal bool IsTextViewHostInitialized => _hasInitializeBeenCalled;

        public TextViewHost(ITextView textView, bool setFocus, TextEditorFactoryService factory, bool initialize = true)
        {
            InputMethod.SetIsInputMethodSuspended(this, true);
            Name = nameof(TextViewHost);
            _textView = textView;
            _setFocus = setFocus;
            _factory = factory;
            if (!initialize)
                return;
            Initialize();
        }

        public void Close()
        {
            if (_isClosed)
                throw new InvalidOperationException();
            Resources.MergedDictionaries.Clear();
            MouseWheel -= OnMouseWheel;
            _textView.BackgroundBrushChanged -= OnBackgroundBrushChanged;
            foreach (var edge in _edges)
                edge.Dispose();
            _textView.Close();
            _isClosed = true;
            _factory.GuardedOperations.RaiseEvent(this, Closed);
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (marginName == null)
                throw new ArgumentNullException(nameof(marginName));
            if (!_hasInitializeBeenCalled)
                throw new InvalidOperationException("The margnins of the text view host have bot been initialized yet.");
            return _edges.Select(edge => edge.GetTextViewMargin(marginName)).FirstOrDefault(textViewMargin => textViewMargin != null);
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_textView.InLayout)
                return;
            using (_factory.PerformanceBlockMarker.CreateBlock("TextEditor.Scroll.MouseWheel"))
                _wheelHelper.HandleMouseWheelEvent(_textView, sender, e);
        }

        internal void Initialize()
        {
            if(_hasInitializeBeenCalled)
                throw new InvalidOperationException();
            MouseWheel += OnMouseWheel;
            Focusable = false;
            _outerGrid = new Grid();
            var columnDefinition1 = new ColumnDefinition {Width = new GridLength(0.0, GridUnitType.Auto)};
            var columnDefinition2 = new ColumnDefinition();
            var columnDefinition3 = new ColumnDefinition {Width = new GridLength(0.0, GridUnitType.Auto)};
            _outerGrid.ColumnDefinitions.Add(columnDefinition1);
            _outerGrid.ColumnDefinitions.Add(columnDefinition2);
            _outerGrid.ColumnDefinitions.Add(columnDefinition3);
            var rowDefinition1 = new RowDefinition {Height = new GridLength(0.0, GridUnitType.Auto)};
            var rowDefinition2 = new RowDefinition();
            var rowDefinition3 = new RowDefinition {Height = new GridLength(0.0, GridUnitType.Auto)};
            _outerGrid.RowDefinitions.Add(rowDefinition1);
            _outerGrid.RowDefinitions.Add(rowDefinition2);
            _outerGrid.RowDefinitions.Add(rowDefinition3);
            _outerGrid.Background = _textView.Background;
            _outerGrid.ShowGridLines = false;
            _textView.BackgroundBrushChanged += OnBackgroundBrushChanged;
            Content = _outerGrid;
            PlaceEdge(0, 1, 1, LeftMargin.Create(this, _factory.GuardedOperations, _factory.MarginState));
            PlaceEdge(2, 1, 1, ContainerMargin.Create("Right", Orientation.Vertical, this, _factory.GuardedOperations, _factory.MarginState));
            PlaceEdge(0, 0, 3, ContainerMargin.Create("Top", Orientation.Horizontal, this, _factory.GuardedOperations, _factory.MarginState));
            PlaceEdge(0, 2, 2, ContainerMargin.Create("Bottom", Orientation.Horizontal, this, _factory.GuardedOperations, _factory.MarginState));
            PlaceEdge(2, 2, 1, ContainerMargin.Create("BottomRightCorner", Orientation.Horizontal, this, _factory.GuardedOperations, _factory.MarginState));
            Grid.SetRow(_textView.VisualElement, 1);
            Grid.SetColumn(_textView.VisualElement, 1);
            _outerGrid.Children.Add(_textView.VisualElement);
            if (_editorResources == null)
                _editorResources = LoadResourceValue<ResourceDictionary>("Themes/Generic/EditorResources.xaml");
            _hasInitializeBeenCalled = true;
            if (!_setFocus)
                return;
            SetFocusToViewLater();
        }

        private void OnBackgroundBrushChanged(object sender, BackgroundBrushChangedEventArgs e)
        {
            _outerGrid.Background = e.NewBackgroundBrush;
        }

        private void PlaceEdge(int column, int row, int colSpan, ITextViewMargin margin)
        {
            Grid.SetColumnSpan(margin.VisualElement, colSpan);
            Grid.SetColumn(margin.VisualElement, column);
            Grid.SetRow(margin.VisualElement, row);
            _outerGrid.Children.Add(margin.VisualElement);
            _edges.Add(margin);
            margin.VisualElement.PreviewMouseDown += OnMarginPreviewMouseDown;
        }

        private void OnMarginPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_textView.IsClosed || IsFocusedElementBetween(sender, e.OriginalSource as DependencyObject))
                return;
            SetFocusToViewLater();
        }

        private static bool IsFocusedElementBetween(object parent, DependencyObject descendant)
        {
            for (; parent != descendant && descendant != null; descendant = LogicalTreeHelper.GetParent(descendant) ?? VisualTreeHelper.GetParent(descendant))
            {
                if (descendant is UIElement uiElement && uiElement.Focusable)
                    return true;
            }
            return false;
        }

        private void SetFocusToViewLater()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() =>
            {
                if (_textView.IsClosed)
                    return;
                _textView.VisualElement.Focus();
            }));
        }

        internal static T LoadResourceValue<T>(string xamlName)
        {
            return (T) Application.LoadComponent(new Uri(
                "/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/" + xamlName, UriKind.Relative));
        }
    }
}
