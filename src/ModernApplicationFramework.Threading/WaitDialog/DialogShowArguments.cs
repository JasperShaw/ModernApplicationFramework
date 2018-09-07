using System;

namespace ModernApplicationFramework.Threading.WaitDialog
{
    public class DialogShowArguments : DialogUpdateArguments
    {
        public string Caption { get; set; }

        public bool IsProgressVisible { get; set; }

        public bool ShowMarqueeProgress { get; set; }

        public string RootWindowCaption { get; set; }

        public IntPtr ActiveWindowHandle { get; set; }

        public DialogShowArguments()
        {
        }

        public DialogShowArguments(string caption, string waitMessage, string progressMessage, bool isCancellable, bool showProgress, int currentStepCount, int totalStepCount)
            : base(waitMessage, progressMessage, isCancellable, currentStepCount, totalStepCount)
        {
            IsProgressVisible = showProgress;
            ShowMarqueeProgress = totalStepCount <= 0;
            Caption = caption;
        }

        public void SetActiveWindowArgs(string rootWindowCaption, IntPtr activeWindowHandle)
        {
            RootWindowCaption = rootWindowCaption;
            ActiveWindowHandle = activeWindowHandle;
        }
    }
}