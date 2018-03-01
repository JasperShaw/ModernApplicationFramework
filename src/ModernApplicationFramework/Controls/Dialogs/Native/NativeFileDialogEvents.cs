using System;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.Controls.Dialogs.Native
{
    internal class NativeFileDialogEvents : IFileDialogEvents, IFileDialogControlEvents
    {
        private readonly NativeFileDialog _dialog;

        public NativeFileDialogEvents(NativeFileDialog dialog)
        {
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        }

        public void OnButtonClicked(IFileDialogCustomize pfdc, int dwIDCtl)
        {
        }

        public void OnCheckButtonToggled(IFileDialogCustomize pfdc, int dwIDCtl, bool bChecked)
        {
        }

        public void OnControlActivating(IFileDialogCustomize pfdc, int dwIDCtl)
        {
        }

        public HResult OnFileOk(IFileDialog pfd)
        {
            return _dialog.DoFileOk(pfd) ? HResult.SOk : HResult.SFalse;
        }

        public void OnFolderChange(IFileDialog pfd)
        {
        }

        public HResult OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
        {
            return HResult.SOk;
        }

        public void OnItemSelected(IFileDialogCustomize pfdc, int dwIDCtl, int dwIDItem)
        {
        }

        public void OnOverwrite(IFileDialog pfd, IShellItem psi, out FdeOverwriteResponse pResponse)
        {
            pResponse = FdeOverwriteResponse.FdeorDefault;
        }

        public void OnSelectionChange(IFileDialog pfd)
        {
        }

        public void OnShareViolation(IFileDialog pfd, IShellItem psi, out FdeShareviolationResponse pResponse)
        {
            pResponse = FdeShareviolationResponse.FdesvrDefault;
        }

        public void OnTypeChange(IFileDialog pfd)
        {
        }
    }
}