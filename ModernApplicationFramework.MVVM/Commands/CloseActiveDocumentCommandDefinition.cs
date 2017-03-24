using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(DefinitionBase))]
    public class CloseActiveDocumentCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => false;
        public override string IconId => null;
        public override Uri IconSource => null;
        public override string Name => "Close File";
        public override string Text => Name;
        public override string ToolTip => Name;

        public override ICommand Command { get; }

        public CloseActiveDocumentCommandDefinition()
        {
            Command = new CommandWrapper(CloseFile, CanCloseFile);
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
