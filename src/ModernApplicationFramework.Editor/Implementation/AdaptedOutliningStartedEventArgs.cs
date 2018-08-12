using System;

namespace ModernApplicationFramework.Editor.Implementation
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