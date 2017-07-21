using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class OpenFileCommandDefinition : CommandDefinition
    {
        public OpenFileCommandDefinition()
        {
            Command = new GestureCommandWrapper(OpenFile, CanOpenFile, new KeyGesture(Key.O, ModifierKeys.Control));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;

        public override ICommand Command { get; }
        public override string IconId => "OpenFile";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/OpenFile_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Open File";
        public override string Text => Name;
        public override string ToolTip => "Opens an File";

        private bool CanOpenFile()
        {
            return _editorProvider.SupportedFileDefinitions.Any();
        }

        private async void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "All Supported Files|"
                         + string.Join(";",
                             _editorProvider.SupportedFileDefinitions.Select(x => x.FileType)
                                            .Select(x => "*" + x.FileExtension))
            };


            dialog.Filter += "|" + string.Join("|", _editorProvider.SupportedFileDefinitions
                                                                   .Select(x => x.FileType)
                                                                   .Select(x => x.Name + "|*" + x.FileExtension));

            if (dialog.ShowDialog() != true)
                return;
            var supportedFileDefinition = _editorProvider.SupportedFileDefinitions
                                                         .FirstOrDefault(
                                                             x =>
                                                                 x.FileType.FileExtension
                                                                 == Path.GetExtension(dialog.FileName));
            var editorType = supportedFileDefinition?.PrefferedEditor;

            _shell.DockingHost.OpenDocument(await EditorProviderHelper.GetEditor(dialog.FileName, editorType));
        }
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;

        [Import] private IEditorProvider _editorProvider;
#pragma warning restore 649
    }
}