using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal class MouseWheelHelper
    {
        private WeakReference _lastSender = new WeakReference(null);
        private int _accumulatedMouseDelta;
        private bool _lastScrollByPages;

        public void HandleMouseWheelEvent(ITextView view, object sender, MouseWheelEventArgs e)
        {
            if (_lastSender.Target != sender)
            {
                _lastSender = new WeakReference(sender);
                _accumulatedMouseDelta = 0;
            }
            if (_accumulatedMouseDelta > 0 != e.Delta > 0)
                _accumulatedMouseDelta = 0;
            var flag = SystemParameters.WheelScrollLines == -1;
            if (_lastScrollByPages != flag)
            {
                _lastScrollByPages = flag;
                _accumulatedMouseDelta = 0;
            }
            _accumulatedMouseDelta += e.Delta;
            var pages = _accumulatedMouseDelta / 120;
            if (pages != 0)
            {
                _accumulatedMouseDelta -= pages * 120;
                if (flag)
                    ScrollByPages(view, pages);
                else
                    ScrollByLines(view, pages * SystemParameters.WheelScrollLines);
            }
            e.Handled = true;
        }

        public static void ScrollByPages(ITextView view, int pages)
        {
            var viewScroller = view.ViewScroller;
            for (var index = Math.Abs(pages); index > 0; --index)
                viewScroller.ScrollViewportVerticallyByPage(pages > 0 ? ScrollDirection.Up : ScrollDirection.Down);
        }

        public static void ScrollByLines(ITextView view, int lines)
        {
            view.ViewScroller.ScrollViewportVerticallyByPixels(lines * view.LineHeight);
        }
    }
}