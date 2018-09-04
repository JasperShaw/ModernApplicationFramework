using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal sealed class ZoomControlMargin : ZoomControl, ITextViewMargin
    {
        private readonly ITextView _view;
        private bool _isDisposed;

        public bool Enabled
        {
            get
            {
                if (_view.Options.IsHorizontalScrollBarEnabled())
                    return _view.Options.IsZoomControlEnabled();
                return false;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return ActualWidth;
            }
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        private int MaxLength
        {
            set
            {
                if (!(GetTemplateChild("PART_EditableTextBox") is TextBox templateChild))
                    return;
                templateChild.MaxLength = value;
            }
        }

        internal ZoomControlMargin(ITextView textView)
        {
            _view = textView;
            _view.Options.OptionChanged += OnOptionChanged;
            IsVisibleChanged += (o, e) =>
            {
                if (IsVisible)
                {
                    if (_view.IsClosed)
                        return;
                    SelectedZoomLevel = _view.ZoomLevel;
                    _view.ZoomLevelChanged += OnZoomLevelChanged;
                }
                else
                {
                    _view.ZoomLevelChanged -= OnZoomLevelChanged;
                }
            };
            InitializeComponents();
            if (!(Application.Current.TryFindResource("MafComboBoxStyleKey") is Style bs))
                SetResourceReference(StyleProperty, typeof(ZoomControl));
            else
            {
                var originalStyle = Application.Current.TryFindResource(typeof(ZoomControl)) as Style;
                if (originalStyle == null)
                {
                    SetResourceReference(StyleProperty, typeof(ZoomControl));
                    return;
                }

                var ns = new Style(typeof(ZoomControl), bs);
                foreach (var originalStyleSetter in originalStyle.Setters)
                    ns.Setters.Add(originalStyleSetter);
                SetValue(StyleProperty, ns);
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _view.Options.OptionChanged -= OnOptionChanged;
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (!_isDisposed && string.Compare(marginName, "ZoomControl", StringComparison.OrdinalIgnoreCase) == 0)
                return this;
            return null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MaxLength = 5;
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            if (SelectedItem == null)
                return;
            ApplyZoom();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                ApplyZoom();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Reset();
                Keyboard.Focus(_view.VisualElement);
            }

            base.OnKeyDown(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!IsFocused)
            {
                e.Handled = true;
                Reset();
            }

            base.OnLostKeyboardFocus(e);
        }

        private void ApplyZoom()
        {
            var bindingExpression = GetBindingExpression(TextProperty);
            bindingExpression.UpdateSource();
            if (bindingExpression.HasError)
                Reset();
            _view.Options.GlobalOptions.SetOptionValue(DefaultViewOptions.ZoomLevelId, SelectedZoomLevel);
            if (Math.Abs(SelectedZoomLevel - _view.ZoomLevel) > 1E-05)
                SelectedZoomLevel = _view.ZoomLevel;
            Keyboard.Focus(_view.VisualElement);
        }

        private void InitializeComponents()
        {
            IsSynchronizedWithCurrentItem = false;
            var zoomLevelConverter = new ZoomLevelConverter();
            BindingOperations.SetBinding(this, TextProperty, new Binding
            {
                Path = new PropertyPath("SelectedZoomLevel", Array.Empty<object>()),
                Mode = BindingMode.TwoWay,
                RelativeSource = RelativeSource.Self,
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                Converter = zoomLevelConverter,
                ValidationRules =
                {
                    new ZoomLevelValidationRule()
                }
            });
        }

        private void OnOptionChanged(object sender, EventArgs e)
        {
            Visibility = Enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnZoomLevelChanged(object sender, ZoomLevelChangedEventArgs e)
        {
            SelectedZoomLevel = e.NewZoomLevel;
        }

        private void Reset()
        {
            GetBindingExpression(TextProperty).UpdateTarget();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("ZoomControl");
        }
    }
}