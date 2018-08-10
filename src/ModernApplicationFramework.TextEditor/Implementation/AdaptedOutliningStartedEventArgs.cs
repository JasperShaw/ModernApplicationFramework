using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class AdaptedOutliningStartedEventArgs : EventArgs
    {
        internal bool RemoveAdhoc { get; }

        internal AdaptedOutliningStartedEventArgs(bool removeAdhoc)
        {
            RemoveAdhoc = removeAdhoc;
        }
    }
}