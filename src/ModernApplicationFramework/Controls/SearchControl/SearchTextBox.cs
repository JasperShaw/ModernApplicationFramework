using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchTextBox : TextBox
    {
        public static readonly DependencyProperty FocusRootProperty = DependencyProperty.Register(
            "FocusRoot", typeof(UIElement), typeof(SearchTextBox), new PropertyMetadata(default(UIElement)));

        private ChangeSelectionData _changeSelection;

        public UIElement FocusRoot
        {
            get => (UIElement) GetValue(FocusRootProperty);
            set => SetValue(FocusRootProperty, value);
        }

        internal bool IsSelectionDelegated { get; set; }

        internal bool IsProcessingMouseMove { get; private set; }

        internal bool IsProcessingMouseDown { get; private set; }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                IsProcessingMouseMove = true;
                base.OnMouseMove(e);
            }
            finally
            {
                IsProcessingMouseMove = false;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            try
            {
                IsProcessingMouseDown = true;
                base.OnMouseDown(e);
            }
            finally
            {
                IsProcessingMouseDown = false;
            }
            if (Mouse.Captured != null || _changeSelection == null)
                return;
            _changeSelection.TrySelectAll();
            _changeSelection = null;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            if (_changeSelection == null)
                return;
            _changeSelection.TrySelectAll();
            _changeSelection = null;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (SelectionLength != 0)
                return;
            Select(0, Text.Length);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (Equals(e.OriginalSource, this))
            {
                var flag = true;
                var element = FocusRoot ?? this;

                if (e.OldFocus is DependencyObject oldFocus)
                {
                    if (IsSelectionDelegated || element.IsLogicalAncestorOf(oldFocus))
                        flag = false;
                    else
                    {
                        var ancestorOrSelf =
                            oldFocus.FindAncestorOrSelf<Popup, DependencyObject>(ExtensionMethods
                                .GetVisualOrLogicalParent);
                        if (ancestorOrSelf?.PlacementTarget != null && element.IsLogicalAncestorOf(ancestorOrSelf.PlacementTarget))
                            flag = false;
                    }
                }

                if (flag)
                {
                    if (!IsProcessingMouseDown)
                        Select(0, Text.Length);
                    else
                        _changeSelection = new ChangeSelectionData(this);
                }
            }
            base.OnGotKeyboardFocus(e);
        }


        private class ChangeSelectionData
        {
            private readonly int _selectionStart;
            private readonly int _selectionLength;
            private readonly SearchTextBox _searchBox;

            public ChangeSelectionData(SearchTextBox searchBox)
            {
                Validate.IsNotNull(searchBox, nameof(searchBox));
                _searchBox = searchBox;
                _selectionStart = searchBox.SelectionStart;
                _selectionLength = searchBox.SelectionLength;
            }

            public bool TrySelectAll()
            {
                if ((_selectionStart != _searchBox.SelectionStart ||
                     _selectionLength != _searchBox.SelectionLength) && _searchBox.SelectionLength != 0 ||
                    _searchBox.IsProcessingMouseMove)
                    return false;
                _searchBox.Select(0, _searchBox.Text.Length);
                return true;
            }
        }


        
    }
}
