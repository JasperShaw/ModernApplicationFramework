using System;
using ModernApplicationFramework.Docking.Controls;

namespace ModernApplicationFramework.Docking
{
    public class FloatingWindowEventArgs : EventArgs
    {
        public FloatingWindowEventArgs(LayoutFloatingWindowControl window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public LayoutFloatingWindowControl Window { get; }
    }
}