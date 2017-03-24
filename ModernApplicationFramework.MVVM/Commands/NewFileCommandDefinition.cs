using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Controls;
using ModernApplicationFramework.MVVM.Core.CommandArguments;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class NewFileCommandDefinition : CommandDefinition
    {
        public NewFileCommandDefinition()
        {
            Command = new GestureCommandWrapper(CreateNewFile, CanCreateNewFile,
                new KeyGesture(Key.N, ModifierKeys.Control));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        public override string IconId => "NewFileIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/NewFile_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "New File";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanCreateNewFile()
        {
            return _editorProvider.SupportedFileDefinitions.Any();
        }

        private void CreateNewFile()
        {
            var vm = (INewElementDialogModel) IoC.GetInstance(typeof(INewElementDialogModel), null);

            vm.ItemPresenter = new FileExtensionItemPresenter {ItemSource = _editorProvider.SupportedFileDefinitions};
            vm.DisplayName = "New File";

            var windowManager = IoC.Get<IWindowManager>();
            if (windowManager.ShowDialog(vm) != true)
                return;

            var ca = vm.ResultData as NewFileCommandArguments;
            if (ca == null)
                return;

            var editor = _editorProvider?.Create(ca.PreferredEditor);
            var viewAware = (IViewAware) editor;
            if (viewAware != null)
                viewAware.ViewAttached += (sender, e) =>
                {
                    var frameworkElement = (FrameworkElement) e.View;

                    async void LoadedHandler(object sender2, RoutedEventArgs e2)
                    {
                        frameworkElement.Loaded -= LoadedHandler;
                        await _editorProvider.New((IStorableDocument) editor, ca.FileName + ca.FileExtension);
                    }

                    frameworkElement.Loaded += LoadedHandler;
                };
            _shell.DockingHost.OpenDocument(editor);
        }
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
        [Import] private IEditorProvider _editorProvider;
#pragma warning restore 649
    }
}