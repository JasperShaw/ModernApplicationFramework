using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.Controls.Dialogs.Native
{
    internal static class ComDlgResources
    {
        public enum ComDlgResourceId
        {
            OpenButton = 370,
            Open = 384,
            FileNotFound = 391,
            CreatePrompt = 402,
            ReadOnly = 427,
            ConfirmSaveAs = 435
        }

        private static readonly Win32Resources Resources = new Win32Resources("comdlg32.dll");

        public static string LoadString(ComDlgResourceId id)
        {
            return Resources.LoadString((uint)id);
        }

        public static string FormatString(ComDlgResourceId id, params string[] args)
        {
            return Resources.FormatString((uint)id, args);
        }
    }
}
