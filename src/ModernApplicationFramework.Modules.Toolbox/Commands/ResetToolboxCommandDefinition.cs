using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ResetToolboxCommandDefinition))]
    public class ResetToolboxCommandDefinition : CommandDefinition<IResetToolboxCommand>
    {
        public override string NameUnlocalized => "Reset";
        public override string Text => "Reset";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{BF0ED4C1-518C-4B30-8FD3-2085A19C63D2}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IResetToolboxCommand : ICommandDefinitionCommand
    {
    }

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
            _serializer.ResetFromBackup(_backupProvider.Backup);
        }
    }
}
