using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Core.CommandArguments;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(DefinitionBase))]
    public class OpenWithFileCommandDefinition : CommandDefinition
    {
        public OpenWithFileCommandDefinition()
        {
            Command = new CommandWrapper(OpenFile, CanOpenFile);
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;

        public override ICommand Command { get; }
        public override string IconId => "OpenFileWith";

        public override Uri IconSource => null;

        public override string Name => "Open File With...";
        public override string Text => Name;
        public override string ToolTip => "Opens File with user defined Type";

        public override object CommandParamenter { get; set; }

        private bool CanOpenFile()
        {
            return _editorProvider.SupportedFileDefinitions.Any();
        }

        private async void OpenFile()
        {
            var arguments = CommandParamenter as OpenFileWithCommandArguments;
            if (arguments == null)
                return;

            if (!File.Exists(arguments.FullFileName))
                return;
            if (!arguments.Editor.GetInterfaces().Contains(typeof(IDocument)))
                return;
            _shell.DockingHost.OpenDocument(await EditorProviderHelper.GetEditor(arguments.FullFileName, arguments.Editor));
            CommandParamenter = null;
        }
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;

        [Import] private IEditorProvider _editorProvider;
#pragma warning restore 649
    }
}