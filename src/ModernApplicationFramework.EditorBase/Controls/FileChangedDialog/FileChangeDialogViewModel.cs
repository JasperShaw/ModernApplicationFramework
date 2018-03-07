using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.Controls.FileChangedDialog
{
    public sealed class FileChangeDialogViewModel : Screen
    {
        private ReloadFileDialogResult Result { get; set; }

        public bool ShowSettingsMessage { get; }

        public string FilePath { get; }

        public string Message { get; }

        public ICommand YesCommand => new DelegateCommand(o =>
        {
            Result = ReloadFileDialogResult.Yes;
            TryClose(true);
        });
        public ICommand YesAllCommand => new DelegateCommand(o =>
        {
            Result = ReloadFileDialogResult.YesAll;
            TryClose(true);
        });
        public ICommand NoCommand => new DelegateCommand(o =>
        {
            Result = ReloadFileDialogResult.No;
            TryClose(true);
        });
        public ICommand NoAllCommand => new DelegateCommand(o =>
        {
            Result = ReloadFileDialogResult.NoAll;
            TryClose(true);
        });


        public FileChangeDialogViewModel(IFile file)
        {
            DisplayName = IoC.Get<IEnvironmentVariables>().ApplicationName;
            FilePath = file.FullFilePath;
            if (file is IStorableFile storableFile && storableFile.IsDirty)
            {
                ShowSettingsMessage = false;
                Message = FileChangedDialogResources.MessageDirty;
            }
            else
            {
                ShowSettingsMessage = true;
                Message = FileChangedDialogResources.MessageClean;
            }
        }

        public ReloadFileDialogResult ShowDialog()
        {
            var result = ReloadFileDialogResult.No;
            OnUIThread(() =>
            {
                result = IoC.Get<IWindowManager>().ShowDialog(this) == true ? Result : ReloadFileDialogResult.No;
            });
            return result;
        }
    }
}
