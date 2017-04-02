using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class CloseActiveDocumentCommandDefinition : CommandDefinition
    {
        [Import] private IDockingMainWindowViewModel _shell;

        public override string IconId => null;
        public override Uri IconSource => null;
        public override string Name => "File.CloseFile";
        public override string Text => "Close File";
        public override string ToolTip => Text;

        public override ICommand Command { get; }

        public CloseActiveDocumentCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(CloseFile, CanCloseFile);
        }

        private bool CanCloseFile()
        {
            return _shell.DockingHost.ActiveItem != null;
        }

        private void CloseFile()
        {
            _shell.DockingHost.ActiveItem.TryClose(true);
        }
    }
}
