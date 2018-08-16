using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Accessibility;
using ModernApplicationFramework.Modules.Editor.Implementation;
using ModernApplicationFramework.Modules.Editor.NativeMethods;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class CaretAdornmentLayer
    {
        private double _focusedOpacity = 1.0;
        internal readonly ITextView TextView;
        private readonly IMultiSelectionBroker _multiSelectionBroker;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly IClassificationFormatMap _classificationFormatMap;
        private Brush _defaultRegularBrush;
        private bool _autoscroll;
        private bool _shouldCaretsBeRendered;
        private bool _viewHasKeyboardFocus;
        private bool _caretsAreBeingRendered;
        private int _blinkInterval;
        private DispatcherTimer _blinkTimer;
        internal Win32Caret Win32Caret;
        internal AccessibleCaret AccessibleCaret;
        internal AccessibleCaret.Element AccessibleCaretElement;
        private HwndSource _hwndSource;
        private HwndSourceHook _hwndSourceHook;
        private MultiSelectionMouseState _mouseState;
        internal Brush TextBrush;
        internal Brush PrimaryCaretBrush;
        internal Brush SecondaryCaretBrush;

        private MultiSelectionMouseState MouseState => _mouseState ?? (_mouseState = MultiSelectionMouseState.GetStateForView(TextView));

        private double FocusedOpacity
        {
            get => _focusedOpacity;
            set
            {
                if (value == _focusedOpacity)
                    return;
                _focusedOpacity = value;
                _adornmentLayer.Opacity = _caretsAreBeingRendered ? _focusedOpacity : 0.0;
            }
        }

        public CaretAdornmentLayer(MultiSelectionAdornmentProvider multiSelectionAdornmentProvider, ITextView textView)
        {
            TextView = textView;
            textView.VisualElement.IsKeyboardFocusedChanged += OnKeyboardFocusChanged;
            _multiSelectionBroker = textView.GetMultiSelectionBroker();
            _editorFormatMap = multiSelectionAdornmentProvider.EditorFormatMapService.GetEditorFormatMap(textView);
            _adornmentLayer = textView.GetAdornmentLayer("Caret");
            _classificationFormatMap = multiSelectionAdornmentProvider.ClassificationFormatMappingService.GetClassificationFormatMap(textView);
            _autoscroll = textView.Options.IsAutoScrollEnabled();
            _shouldCaretsBeRendered = textView.Options.GetOptionValue(DefaultTextViewOptions.ShouldCaretsBeRenderedId);
            textView.LayoutChanged += OnLayoutChanged;
            textView.Options.OptionChanged += OnOptionChanged;
            _multiSelectionBroker.MultiSelectionSessionChanged += OnMultiSelectionSessionChanged;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
            textView.Closed += OnTextView_Closed;
            MouseState.ProvisionalSelectionChanged += OnProvisionalSelectionChanged;
            Win32Caret = new Win32Caret(textView, _multiSelectionBroker);
            AccessibleCaret = new AccessibleCaret(Win32Caret, _multiSelectionBroker, textView);
            AccessibleCaretElement = new AccessibleCaret.Element(Win32Caret);
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, new SnapshotSpan?(), null, AccessibleCaretElement, null);
            UpdateDefaultBrushes();
            UpdatePrimaryCaretBrushes();
            UpdateSecondaryCaretBrushes();
            UpdateTextBrushes();
            OnKeyboardFocusChanged(null, new DependencyPropertyChangedEventArgs());
            _blinkInterval = -2;
            UpdateCaretVisibility(true);
        }

        private void UpdateCaretVisibility(bool forceUpdate)
        {
            var flag = _viewHasKeyboardFocus && _shouldCaretsBeRendered;
            if (!(flag != _caretsAreBeingRendered | forceUpdate))
                return;
            _caretsAreBeingRendered = flag;
            if (flag)
            {
                RedrawCarets();
                FocusedOpacity = 1.0;
                _blinkTimer?.Start();
            }
            else
            {
                _blinkTimer?.Stop();
                _adornmentLayer.RemoveAdornmentsByTag("Visible Caret");
            }
        }

        private void RedrawCarets()
        {
            if (!_caretsAreBeingRendered || TextView.InLayout || TextView.IsClosed)
                return;
            TrySetupHwndHook();
            var textViewLines = TextView.TextViewLines;
            if (textViewLines == null)
                return;
            _adornmentLayer.RemoveAdornmentsByTag("Visible Caret");
            var provisionalSelection = MouseState.ProvisionalSelection;
            foreach (var region in _multiSelectionBroker.GetSelectionsIntersectingSpan(textViewLines.FormattedSpan))
            {
                if (provisionalSelection == Selection.Invalid || !provisionalSelection.Extent.IntersectsWith(region.Extent))
                    DrawCaretForRegion(region);
            }
        }

        private void DrawCaretForRegion(Selection region)
        {
            var visualSpan = new SnapshotSpan(region.InsertionPoint.Position, 0);
            if (TextView.TextViewLines.GetTextViewLineContainingBufferPosition(region.InsertionPoint.Position) == null || !_multiSelectionBroker.TryGetSelectionPresentationProperties(region, out var properties))
                return;
            var caret = new Caret(this, region, properties);
            _adornmentLayer.AddAdornment(visualSpan, "Visible Caret", caret);
        }

        private void TrySetupHwndHook()
        {
            if (_hwndSource != null)
                return;
            _hwndSource = (HwndSource)PresentationSource.FromVisual(AccessibleCaretElement);
            if (_hwndSource == null)
                return;
            _hwndSourceHook = HwndHook;
            _hwndSource.AddHook(_hwndSourceHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != 61 || !(lParam != IntPtr.Zero) || (long)lParam != -8L)
                return IntPtr.Zero;
            var iunknownForObject = Marshal.GetIUnknownForObject(AccessibleCaret);
            var guid = typeof(IAccessible).GUID;
            var num = OleAcc.LresultFromObject(ref guid, wParam, iunknownForObject);
            Marshal.Release(iunknownForObject);
            handled = true;
            return num;
        }

        private void UpdateTextBrushes()
        {
            TextBrush = GetBrushForFormat("Plain Text");
        }

        private void UpdateSecondaryCaretBrushes()
        {
            SecondaryCaretBrush = GetBrushForFormat("Caret (Secondary)");
        }

        private void UpdatePrimaryCaretBrushes()
        {
            PrimaryCaretBrush = GetBrushForFormat("Caret (Primary)");
        }

        private Brush GetBrushForFormat(string formatName)
        {
            var properties = _editorFormatMap.GetProperties(formatName);
            var brush = !properties.Contains("ForegroundColor")
                ? (!properties.Contains("Foreground")
                    ? _defaultRegularBrush
                    : (Brush) properties["Foreground"])
                : new SolidColorBrush((Color) properties["ForegroundColor"]);
            if (brush.CanFreeze)
                brush.Freeze();
            return brush;
        }

        private bool UpdateDefaultBrushes()
        {
            var defaultTextProperties = _classificationFormatMap.DefaultTextProperties;
            if (defaultTextProperties.ForegroundBrushEmpty)
            {
                _defaultRegularBrush = SystemColors.WindowTextBrush.Clone();
                return true;
            }

            if (defaultTextProperties.ForegroundBrushSame(_defaultRegularBrush))
                return false;
            _defaultRegularBrush = defaultTextProperties.ForegroundBrush;
            return true;
        }

        private void Invalidate()
        {
            UpdateBlinkTimer();
            RedrawCarets();
        }

        private void UpdateBlinkTimer()
        {
            int caretBlinkTime = CaretBlinkTimeManager.GetCaretBlinkTime();
            if (_blinkInterval == caretBlinkTime)
                return;
            _blinkInterval = caretBlinkTime;
            if (caretBlinkTime > 0)
            {
                if (_blinkTimer == null)
                    _blinkTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, caretBlinkTime), DispatcherPriority.Normal, OnTimerElapsed, TextView.VisualElement.Dispatcher);
                else
                    _blinkTimer.Interval = new TimeSpan(0, 0, 0, 0, caretBlinkTime);
                if (!_caretsAreBeingRendered)
                    return;
                _blinkTimer.Start();
            }
            else
            {
                _blinkTimer?.Stop();
                FocusedOpacity = 1.0;
            }
        }

        private void InvalidateAndDisplay()
        {
            RedrawCarets();
            if (_blinkTimer != null)
            {
                _blinkTimer.Stop();
                _blinkTimer.Start();
            }
            FocusedOpacity = 1.0;
        }

        private void QueueScroll()
        {
            TextView.VisualElement.Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() =>
            {
                if (TextView.IsClosed)
                    return;
                if (TextView.GetInOuterLayout())
                    QueueScroll();
                else
                    _multiSelectionBroker.TryEnsureVisible(_multiSelectionBroker.PrimarySelection, EnsureSpanVisibleOptions.MinimumScroll);
            }));
        }

        private static bool AnyTextChanges(ITextVersion oldVersion, ITextVersion currentVersion)
        {
            for (; oldVersion != currentVersion; oldVersion = oldVersion.Next)
            {
                if (oldVersion.Changes.Count > 0)
                    return true;
            }
            return false;
        }

        private void OnProvisionalSelectionChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnTextView_Closed(object sender, EventArgs e)
        {
            _blinkTimer?.Stop();
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
            _multiSelectionBroker.MultiSelectionSessionChanged -= OnMultiSelectionSessionChanged;
            if (_hwndSource != null && _hwndSourceHook != null)
            {
                _hwndSource.RemoveHook(_hwndSourceHook);
                _hwndSource = null;
                _hwndSourceHook = null;
            }
            Win32Caret.Destroy();
            Win32Caret.Dispose();
            TextView.LayoutChanged -= OnLayoutChanged;
            TextView.Options.OptionChanged -= OnOptionChanged;
            MouseState.ProvisionalSelectionChanged -= OnProvisionalSelectionChanged;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            bool flag = false;
            if (e.ChangedItems.Contains("Plain Text"))
            {
                UpdateTextBrushes();
                flag = true;
            }
            if (e.ChangedItems.Contains("Caret (Primary)"))
            {
                UpdatePrimaryCaretBrushes();
                flag = true;
            }
            if (e.ChangedItems.Contains("Caret (Secondary)"))
            {
                UpdateSecondaryCaretBrushes();
                flag = true;
            }
            if (!flag)
                return;
            RedrawCarets();
        }

        private void OnMultiSelectionSessionChanged(object sender, EventArgs e)
        {
            InvalidateAndDisplay();
        }

        private void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (string.Equals(e.OptionId, "TextView/OverwriteMode", StringComparison.OrdinalIgnoreCase))
            {
                if (!_caretsAreBeingRendered)
                    return;
                InvalidateAndDisplay();
            }
            else if (string.Equals(e.OptionId, "TextView/AutoScroll", StringComparison.OrdinalIgnoreCase))
            {
                _autoscroll = TextView.Options.IsAutoScrollEnabled();
            }
            else
            {
                if (!string.Equals(e.OptionId, "TextView/ShouldCaretsBeRendered", StringComparison.OrdinalIgnoreCase))
                    return;
                _shouldCaretsBeRendered =
                    TextView.Options.GetOptionValue(DefaultTextViewOptions.ShouldCaretsBeRenderedId);
                UpdateCaretVisibility(false);
            }
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            Invalidate();
            if (!_autoscroll || !AnyTextChanges(e.OldSnapshot.Version, e.NewSnapshot.Version) || _multiSelectionBroker.PrimarySelection.InsertionPoint.Position.GetContainingLine().LineNumber != e.NewSnapshot.LineCount - 1)
                return;
            QueueScroll();
        }

        private void OnKeyboardFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewHasKeyboardFocus = TextView.VisualElement.IsKeyboardFocused;
            UpdateCaretVisibility(false);
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            FocusedOpacity = FocusedOpacity == 0.0 ? 1.0 : 0.0;
        }
    }
}
