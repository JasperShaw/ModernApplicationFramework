using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModernApplicationFramework.TextEditor
{
    internal class TextViewHost : ContentControl, ITextViewHost
    {
        private bool _isClosed;
        private readonly bool _setFocus;
        private readonly TextEditorFactoryService _factory;
        private bool _readOnly;
        private bool _hasInitializeBeenCalled;
        private Grid _outerGrid;
        private readonly ITextView _textView;
        private static ResourceDictionary _editorResources;

        public event EventHandler Closed;

        public bool IsClosed => _isClosed;

        public Control HostControl => this;

        public ITextView TextView => _textView;

        public bool IsReadOnly
        {
            get => _readOnly;
            set => _readOnly = value;
        }

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

            _textView.Close();
            _isClosed = true;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {

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
