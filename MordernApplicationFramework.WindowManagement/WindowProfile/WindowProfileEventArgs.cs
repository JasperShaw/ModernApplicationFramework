using System;

namespace MordernApplicationFramework.WindowManagement.WindowProfile
{
    internal class WindowProfileEventArgs : EventArgs
    {
        public WindowProfile WindowProfile { get; }

        public WindowProfileEventArgs(WindowProfile profile)
        {
            WindowProfile = profile;
        }
    }
}
