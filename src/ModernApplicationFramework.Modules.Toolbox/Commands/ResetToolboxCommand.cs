using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IResetToolboxCommand))]
    internal class ResetToolboxCommand : CommandDefinitionCommand, IResetToolboxCommand
    {
        private readonly IToolboxStateSerializer _serializer;
        private readonly IToolboxStateBackupProvider _backupProvider;

        [ImportingConstructor]
        public ResetToolboxCommand(IToolboxStateSerializer serializer, IToolboxStateBackupProvider backupProvider)
        {
            _serializer = serializer;
            _backupProvider = backupProvider;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _backupProvider.Backup != null;
        }

        protected override void OnExecute(object parameter)
        {
            if (ToolboxUserDialogs.AskUserForReset() == MessageBoxResult.Yes)
                _serializer.ResetFromBackup(_backupProvider.Backup);
        }
    }
}