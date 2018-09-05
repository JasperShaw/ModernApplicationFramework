using System;

namespace ModernApplicationFramework.Threading.WaitDialog
{
    public class DialogInitializationArguments
    {
        public string AppName { get; set; }

        public IntPtr AppMainWindowHandle { get; set; }

        public int AppProcessId { get; set; }
    }
}