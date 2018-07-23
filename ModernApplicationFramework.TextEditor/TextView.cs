using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public class TextView : ContentControl, ITextView
    {
        private bool _hasInitializeBeenCalled;
        private bool _isClosed;
        private bool _hasAggregateFocus;
        private bool _hasKeyboardFocus;
        private Action _loadedAction;
        private bool _hasBeenLoaded;
        private Canvas _controlHostLayer;
        private Canvas _manipulationLayer;
        private ViewStack _baseLayer;
        private ViewStack _overlayLayer;
        //private CaretElement _caretElement;
        internal TextContentLayer _contentLayer = new TextContentLayer();

        [ThreadStatic]
        private static TextView ViewWithAggregateFocus;

        public event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;

        public event EventHandler Closed;

        public FrameworkElement VisualElement => this;

        public bool IsClosed => _isClosed;

        public FrameworkElement ManipulationLayer => _manipulationLayer;

        public new Brush Background
        {
            get => _controlHostLayer.Background;
            set
            {
                _controlHostLayer.Background = value;
                BackgroundBrushChanged?.Invoke(this, new BackgroundBrushChangedEventArgs(value));
            }
        }

        public TextView(bool initialize = true)
        {
            if (!initialize)
                return;
            Initialize();
        }

        internal void Initialize()
        {
            if (_hasInitializeBeenCalled)
                throw new InvalidOperationException();
            Name = nameof(TextView);
            AllowDrop = true;

            _manipulationLayer = new Canvas();
            _baseLayer = new ViewStack(new Dictionary<string, int>(), this);
            _overlayLayer = new ViewStack(new Dictionary<string, int>(), this, true);

            InitializeLayers();
            Loaded += OnLoaded;

            _hasInitializeBeenCalled = true;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            if (IsClosed)
                return;
            _hasBeenLoaded = true;
            if (_loadedAction == null)
                return;
            _loadedAction();
            _loadedAction = null;
        }

        public void Close()
        {
            if (_isClosed)
                throw new InvalidOperationException();
            if (_hasAggregateFocus)
            {
                ViewWithAggregateFocus = null;
                _hasAggregateFocus = false;
            }


            _isClosed = true;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeLayers()
        {
            Focusable = true;
            FocusVisualStyle = null;
            Cursor = Cursors.IBeam;

            _manipulationLayer.Background = Brushes.Transparent;
            _baseLayer.TryAddElement("Text", _contentLayer);
            //_baseLayer.TryAddElement("Caret", _caretElement);
            _controlHostLayer = new Canvas();
            _controlHostLayer.Background = GetBackgroundColorFromFormatMap();
            Content = _controlHostLayer;
            _controlHostLayer.SizeChanged += (sender, args) =>
            {
                _manipulationLayer.Width = args.NewSize.Width;
                _manipulationLayer.Height = args.NewSize.Height;
                _baseLayer.SetSize(args.NewSize);
                _overlayLayer.SetSize(args.NewSize);
                if (args.NewSize.Width == 0.0 || args.NewSize.Height == 0.0)
                    _controlHostLayer.Children.Clear();
                else
                {
                    if (_controlHostLayer.Children.Count != 0)
                        return;
                    _controlHostLayer.Children.Add(ManipulationLayer);
                    _controlHostLayer.Children.Add(_baseLayer);
                    _controlHostLayer.Children.Add(_overlayLayer);
                }
            };
        }

        private Brush GetBackgroundColorFromFormatMap()
        {
            return Brushes.White;
        }
    }
}
