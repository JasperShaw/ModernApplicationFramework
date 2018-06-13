using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ResetToolboxCommandDefinition))]
    public class ResetToolboxCommandDefinition : CommandDefinition
    {
        private readonly IToolboxStateSerializer _serializer;
        private readonly IToolboxStateBackupProvider _backupProvider;
        public override string NameUnlocalized => "Reset";
        public override string Text => "Reset";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{BF0ED4C1-518C-4B30-8FD3-2085A19C63D2}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ResetToolboxCommandDefinition(IToolboxStateSerializer serializer, IToolboxStateBackupProvider backupProvider)
        {
            _serializer = serializer;
            _backupProvider = backupProvider;
            Command = new UICommand(ResetToolbox, CanResetToolbox);
        }

        private void ResetToolbox()
        {
            _serializer.ResetFromBackup(_backupProvider.Backup);
        }

        private bool CanResetToolbox()
        {
            return _backupProvider.Backup != null;
        }
    }
}
