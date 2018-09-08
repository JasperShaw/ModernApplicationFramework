using System;
using ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal partial class CommandHandlerServiceAdapter
    {
        private void ExecuteTypeCharCommand(Action next, char typedChar)
        {
            _commandHandlerService.Execute(
                (textView, subjectBuffer) => new TypeCharCommandArgs(TextView, subjectBuffer, typedChar), next);
        }

        private void ExecuteBackspaceKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new BackspaceKeyCommandArgs(view, buffer),
                nextCommandHandler);
        }

        private void ExecuteReturnKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new ReturnKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteLeftKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new LeftKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteRightKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new RightKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteCopyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new CopyCommandArgs(view, buffer), nextCommandHandler);
        }
    }
}