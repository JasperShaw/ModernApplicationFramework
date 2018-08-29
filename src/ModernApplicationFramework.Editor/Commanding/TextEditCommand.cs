using System;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Editor.Commanding
{
    internal class TextEditCommand : CommandDefinitionCommand
    {
        private readonly Guid _commandGroup;
        private readonly uint _commandId;
        private readonly IActiveTextViewState _activeTextViewState;

        public TextEditCommand(MafConstants.EditorCommands command) :
            this(MafConstants.EditorCommandGroup, command)
        {

        }

        public TextEditCommand(Guid commandGroup, MafConstants.EditorCommands command)
        {
            _commandGroup = commandGroup;
            _commandId = (uint)command;
            _activeTextViewState = ActiveTextViewState.Instance;
        }

        protected override bool OnCanExecute(object parameter)
        {
            var commandTarget = _activeTextViewState?.ActiveCommandTarget;
            if (_activeTextViewState?.ActiveCommandTarget == null || commandTarget == null)
                return false;

            var prgCmds = new[]
            {
                new Olecmd{cmdID = _commandId}
            };

            return commandTarget.QueryStatus(_commandGroup, (uint) prgCmds.Length, prgCmds, IntPtr.Zero) >= 0 &&
                   ((int) prgCmds[0].cmdf & 2) != 0;
        }

        protected override void OnExecute(object parameter)
        {
            _activeTextViewState.ActiveCommandTarget.Exec(MafConstants.EditorCommandGroup, _commandId, 0, IntPtr.Zero, IntPtr.Zero);
        }
    }
}