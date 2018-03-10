using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.FileSupport.Exceptions;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.NewElementDialog;
using ModernApplicationFramework.EditorBase.NewElementDialog.ViewModels;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewFileCommandDefinition))]
    public sealed class NewFileCommandDefinition : CommandDefinition
    {
        private IEditorProvider _editorProvider;

        private IEditorProvider EditorProvider => _editorProvider ?? (_editorProvider = IoC.Get<IEditorProvider>());

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "NewFileIcon";

        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public override Guid Id => new Guid("{B33B7AA8-2FB6-4F80-88A2-3F97878273F3}");

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.EditorBase;component/Resources/Icons/VSO_NewFile_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "NewFile";
        public override string NameUnlocalized => "New File";
        public override string Text => CommandsResources.NewFileCommandText;
        public override string ToolTip => Text;

        public NewFileCommandDefinition()
        {
            var command = new UICommand(CreateNewFile, CanCreateNewFile);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(Key.N, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        private bool CanCreateNewFile()
        {
            return EditorProvider.SupportedFileDefinitions.Any();
        }

        private void CreateNewFile()
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
