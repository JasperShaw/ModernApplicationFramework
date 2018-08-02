using System;
using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal class MouseWheelHelper
    {
        private WeakReference _lastSender = new WeakReference((object)null);
        private int _accumulatedMouseDelta;
        private bool _lastScrollByPages;

        public void HandleMouseWheelEvent(ITextView view, object sender, MouseWheelEventArgs e)
        {

        }
    }
}