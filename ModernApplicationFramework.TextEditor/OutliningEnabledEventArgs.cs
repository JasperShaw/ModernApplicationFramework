using System;

namespace ModernApplicationFramework.TextEditor
{
    public class OutliningEnabledEventArgs : EventArgs
    {
        public bool Enabled { get; }

        public OutliningEnabledEventArgs(bool enabled)
        {
            Enabled = enabled;
        }
    }
}