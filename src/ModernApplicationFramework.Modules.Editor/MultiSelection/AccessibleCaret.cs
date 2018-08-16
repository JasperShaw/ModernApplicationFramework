using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Accessibility;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    [ComVisible(true)]
    internal class AccessibleCaret : IAccessible
    {
        private readonly Win32Caret _win32Caret;
        private readonly ITextView _textView;

        public AccessibleCaret(Win32Caret w32Caret, IMultiSelectionBroker broker, ITextView textView)
        {
            _win32Caret = w32Caret;
            var broker1 = broker;
            _textView = textView;
            broker1.MultiSelectionSessionChanged += OnMultiSelectionSessionChanged;
            broker1.TextView.VisualElement.IsVisibleChanged += OnViewVisibilityChanged;
        }

        private void OnViewVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_textView.IsClosed || _textView.InLayout)
                return;
            if (!(bool)e.NewValue)
            {
                if (!_win32Caret.IsVisible)
                    return;
                _win32Caret.Hide();
            }
            else
            {
                if (!_win32Caret.IsShownOnScreen)
                    return;
                if (_win32Caret.IsVisible)
                    _win32Caret.Update();
                else
                    _win32Caret.Show();
            }
        }

        private void OnMultiSelectionSessionChanged(object sender, EventArgs e)
        {
            if (_textView.IsClosed || _textView.InLayout || !_win32Caret.IsShownOnScreen)
                return;
            if (_win32Caret.IsVisible)
                _win32Caret.Update();
            else
                _win32Caret.Show();
        }

        public int accChildCount => 0;

        public void accDoDefaultAction(object varChild = null)
        {
        }

        public object accFocus => null;

        public object accHitTest(int xLeft, int yTop)
        {
            return null;
        }

        public void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, object varChild = null)
        {
            varChild = 0;
            var topLeft = _win32Caret.TopLeft;
            var bottomRight = _win32Caret.BottomRight;
            pxLeft = (int)Math.Round(topLeft.X);
            pyTop = (int)Math.Round(topLeft.Y);
            pcxWidth = (int)Math.Round(bottomRight.X - topLeft.X);
            pcyHeight = (int)Math.Round(bottomRight.Y - topLeft.Y);
        }

        public object accNavigate(int navDir, object varStart = null)
        {
            throw new NotImplementedException();
        }

        public object accParent => 1;

        public void accSelect(int flagsSelect, object varChild = null)
        {
        }

        public object accSelection => null;

        public object get_accChild(object varChild)
        {
            return null;
        }

        public string get_accDefaultAction(object varChild = null)
        {
            return null;
        }

        public string get_accDescription(object varChild = null)
        {
            return "The caret enables moving and editing of text across all locations of the editor";
        }

        public string get_accHelp(object varChild = null)
        {
            return "The caret enables moving and editing of text across all locations of the editor";
        }

        public int get_accHelpTopic(out string pszHelpFile, object varChild = null)
        {
            throw new NotImplementedException();
        }

        public string get_accKeyboardShortcut(object varChild = null)
        {
            throw new NotImplementedException();
        }

        public string get_accName(object varChild = null)
        {
            return "Edit";
        }

        public object get_accRole(object varChild = null)
        {
            return 7;
        }

        public object get_accState(object varChild = null)
        {
            if (_win32Caret.IsShownOnScreen)
                return 0;
            return 32768;
        }

        public string get_accValue(object varChild = null)
        {
            return null;
        }

        public void set_accName(object varChild, string pszName)
        {
            throw new NotImplementedException();
        }

        public void set_accValue(object varChild, string pszValue)
        {
            throw new NotImplementedException();
        }

        internal class Element : UIElement
        {
            private readonly Win32Caret _win32Caret;

            public Element(Win32Caret win32Caret)
            {
                _win32Caret = win32Caret;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                if (!_win32Caret.IsShownOnScreen)
                    return;
                if (_win32Caret.IsVisible)
                    _win32Caret.Update();
                else
                    _win32Caret.Show();
            }
        }
    }
}
