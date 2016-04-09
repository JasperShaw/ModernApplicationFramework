using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Caliburn.Interfaces;
using ModernApplicationFramework.Caliburn.Platform.Xaml;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.ViewModels;
using ModernApplicationFramework.MVVM.Views;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(CommandDefinition))]
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
            //TODO: Set Itemsource
            vm.ItemPresenter = new NewFileItemPresenter();
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
                    RoutedEventHandler loadedHandler = null;
                    loadedHandler = async (sender2, e2) =>
                    {
                        frameworkElement.Loaded -= loadedHandler;
                        await _editorProvider.New((IStorableDocument) editor, ca.FileName + ca.FileExtension);
                    };
                    frameworkElement.Loaded += loadedHandler;
                };
            _shell.DockingHost.OpenDocument(editor);
        }
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
        [Import] private IEditorProvider _editorProvider;
#pragma warning restore 649
    }
}