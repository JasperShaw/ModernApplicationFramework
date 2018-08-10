using System;

namespace ModernApplicationFramework.Text.Ui.Outlining
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