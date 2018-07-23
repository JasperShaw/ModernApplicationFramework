using System;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class DialogInitializationArguments
    {
        public string AppName { get; set; }

        public IntPtr AppMainWindowHandle { get; set; }

        public int AppProcessId { get; set; }
    }
}