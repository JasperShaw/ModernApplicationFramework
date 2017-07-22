using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class SaveFileAsCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649


        public SaveFileAsCommandDefinition()
        {
            Command = new AbstractCommandWrapper(SaveFile, CanSaveFile);
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;

        public override ICommand Command { get;}
        public override string IconId => null;

        public override Uri IconSource => null;

        public override string Name => "Save As";
        public override string Text => Name;
        public override string ToolTip => "Saves the active file";

        private bool CanSaveFile()
        {
            return _shell.DockingHost.ActiveItem is IStorableDocument;
        }

        private void SaveFile()
        {
            _shell.DockingHost.ActiveItem.SaveFileAsCommand.Execute(null);
        }
    }
}