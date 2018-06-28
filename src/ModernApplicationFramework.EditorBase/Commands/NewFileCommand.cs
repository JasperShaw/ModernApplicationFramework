using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Dialogs.NewElementDialog;
using ModernApplicationFramework.EditorBase.Dialogs.NewElementDialog.ViewModels;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.FileSupport.Exceptions;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(INewFileCommand))]
    internal class NewFileCommand : CommandDefinitionCommand, INewFileCommand
    {
        private IEditorProvider _editorProvider;

        private IEditorProvider EditorProvider => _editorProvider ?? (_editorProvider = IoC.Get<IEditorProvider>());


        protected override bool OnCanExecute(object parameter)
        {
            return EditorProvider.SupportedFileDefinitions.Any();
        }

        protected override void OnExecute(object parameter)
        {
            var vm = new NewElementDialogViewModel<NewFileArguments>();

            var presenter = IoC.Get<INewFileSelectionModel>();
            vm.ItemPresenter = presenter;
            vm.DisplayName = NewElementDialogResources.NewFileDialogWindowTitle;

            var windowManager = IoC.Get<IWindowManager>();
            if (windowManager.ShowDialog(vm) != true)
                return;
            var args = vm.ResultData;
            try
            {
                EditorProvider.New(args);
            }
            catch (FileNotSupportedException exception)
            {
                MessageBox.Show(exception.Message, IoC.Get<IEnvironmentVariables>().ApplicationName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}