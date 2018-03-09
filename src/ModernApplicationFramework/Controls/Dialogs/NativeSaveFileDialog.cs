using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using ModernApplicationFramework.Controls.Dialogs.Native;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.Controls.Dialogs
{
    public class NativeSaveFileDialog : NativeFileDialog
    {
        [DefaultValue(false), Category("Behavior"), Description("A value indicating whether the dialog box prompts the user for permission to create a file if the user specifies a file that does not exist.")]
        public bool CreatePrompt
        {
            get => ((SaveFileDialog) DownlevelDialog)?.CreatePrompt ?? GetOption(Fos.FosCreateprompt);
            set
            {
                if (DownlevelDialog != null)
                    ((SaveFileDialog)DownlevelDialog).CreatePrompt = value;
                else
                    SetOption(Fos.FosCreateprompt, value);
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("A value indicating whether the Save As dialog box displays a warning if the user specifies a file name that already exists.")]
        public bool OverwritePrompt
        {
            get
            {
                if (DownlevelDialog != null)
                    return ((SaveFileDialog)DownlevelDialog).OverwritePrompt;
                return GetOption(Fos.FosOverwriteprompt);
            }
            set
            {
                if (DownlevelDialog != null)
                    ((SaveFileDialog)DownlevelDialog).OverwritePrompt = value;
                else
                    SetOption(Fos.FosOverwriteprompt, value);
            }
        }

        public NativeSaveFileDialog()
        {
            if (!IsVistaFileDialogSupported)
                DownlevelDialog = new SaveFileDialog();
        }

        public override void Reset()
        {
            base.Reset();
            if (DownlevelDialog == null)
            {
                OverwritePrompt = true;
            }
        }

        public Stream OpenFile()
        {
            if (DownlevelDialog != null)
                return ((SaveFileDialog) DownlevelDialog).OpenFile();
            string fileName = FileName;
            return new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
        }

        internal override IFileDialog CreateFileDialog()
        {
            return new NativeFileSaveDialog();
        }

        protected override void OnFileOk(CancelEventArgs e)
        {
            // For reasons unknown, .Net puts the OFN_FILEMUSTEXIST and OFN_CREATEPROMPT flags on the save file dialog despite 
            // the fact that these flags only works on open file dialogs, and then prompts manually. Similarly, the 
            // FOS_CREATEPROMPT and FOS_FILEMUSTEXIST flags don't actually work on IFileSaveDialog, so we have to implement 
            // the prompt manually.
            if (DownlevelDialog == null)
            {
                if (CheckFileExists && !File.Exists(FileName))
                {
                    PromptUser(ComDlgResources.FormatString(ComDlgResources.ComDlgResourceId.FileNotFound, Path.GetFileName(FileName)), MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                    e.Cancel = true;
                    return;
                }
                if (CreatePrompt && !File.Exists(FileName))
                {
                    if (!PromptUser(ComDlgResources.FormatString(ComDlgResources.ComDlgResourceId.CreatePrompt, Path.GetFileName(FileName)), MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            base.OnFileOk(e);
        }
    }
}
