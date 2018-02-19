using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Controls;
using ModernApplicationFramework.EditorBase.Controls.NewElementDialog;
using ModernApplicationFramework.EditorBase.Controls.NewFileExtension;
using ModernApplicationFramework.EditorBase.Core;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Layout;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewFileCommandDefinition))]
    public sealed class NewFileCommandDefinition : CommandDefinition
    {

        private IEditorProvider _editorProvider;

        private IEditorProvider EditorProvider => _editorProvider ?? (_editorProvider = IoC.Get<IEditorProvider>());


        public NewFileCommandDefinition()
        {
            var command = new UICommand(CreateNewFile, CanCreateNewFile);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(Key.N, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        //public override string IconId => "NewFileIcon";
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public override Guid Id => new Guid("{B33B7AA8-2FB6-4F80-88A2-3F97878273F3}");

        public override Uri IconSource => null; 

        //public override Uri IconSource
        //    =>
        //        new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/NewFile_16x.xaml",
        //            UriKind.RelativeOrAbsolute);

        public override string Name => "New File";
        public override string NameUnlocalized => "New File";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanCreateNewFile()
        {
            return EditorProvider.SupportedFileDefinitions.Any();
        }

        private void CreateNewFile()
        {
            new TestWindow().ShowDialog();


            //var vm = new NewElementDialogViewModel<NewFileCommandArguments>();

            //var presenter = IoC.Get<NewFileSelectionScreenViewModel>();
            //presenter.ItemSource = EditorProvider.SupportedFileDefinitions;

            //vm.ItemPresenter = presenter;
            //vm.DisplayName = "New File";

            //var windowManager = IoC.Get<IWindowManager>();
            //if (windowManager.ShowDialog(vm) != true)
            //    return;

            //NewFileCommandArguments result = vm.ResultData;

            //var editor = EditorProvider?.Create(result.PreferredEditor);
            //var viewAware = (IViewAware) editor;
            //if (viewAware != null)
            //    viewAware.ViewAttached += (sender, e) =>
            //    {
            //        var frameworkElement = (FrameworkElement) e.View;

            //        async void LoadedHandler(object sender2, RoutedEventArgs e2)
            //        {
            //            frameworkElement.Loaded -= LoadedHandler;
            //            await EditorProvider.New((IStorableDocument) editor, result.FileName + result.FileExtension);
            //        }

            //        frameworkElement.Loaded += LoadedHandler;
            //    };
            //IoC.Get<IDockingMainWindowViewModel>().DockingHost.OpenDocument(editor);
        }
    }
}