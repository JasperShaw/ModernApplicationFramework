using System;
using System.Runtime.InteropServices;
using Accessibility;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [ComVisible(true)]
    internal class AccessibleCaret : IAccessible
    {
        private readonly CaretElement _owner;
        private readonly CaretElement.Win32Caret _w32Caret;

        public int accChildCount => 0;

        public object accFocus => null;

        public object accParent => 1;

        public object accSelection => null;

        public AccessibleCaret(CaretElement owner, CaretElement.Win32Caret w32Caret)
        {
            _owner = owner;
            _w32Caret = w32Caret;
        }

        public void accDoDefaultAction(object varChild = null)
        {
        }

        public object accHitTest(int xLeft, int yTop)
        {
            return null;
        }

        public void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight,
            object varChild = null)
        {
            var topLeft = _w32Caret.TopLeft;
            var bottomRight = _w32Caret.BottomRight;
            pxLeft = (int) Math.Round(topLeft.X);
            pyTop = (int) Math.Round(topLeft.Y);
            pcxWidth = (int) Math.Round(bottomRight.X - topLeft.X);
            pcyHeight = (int) Math.Round(bottomRight.Y - topLeft.Y);
        }

        public object accNavigate(int navDir, object varStart = null)
        {
            throw new NotImplementedException();
        }

        public void accSelect(int flagsSelect, object varChild = null)
        {
        }

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
            if (_owner.IsShownOnScreen)
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
    }
}