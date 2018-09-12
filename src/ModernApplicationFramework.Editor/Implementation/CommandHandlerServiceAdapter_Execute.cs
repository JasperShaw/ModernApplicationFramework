using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;
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

        private void ExecuteTabKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new TabKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteBackTabKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new BackTabKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteDeleteKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new DeleteKeyCommandArgs(view, buffer), nextCommandHandler);
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

        private void ExecuteUpKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new UpKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteDownKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new DownKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteDocumentEndCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new DocumentEndCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteDocumentStartCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new DocumentStartCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteLineStartCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new LineStartCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteLineStartExtendCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new LineStartExtendCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteLineEndCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new LineEndCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteLineEndExtendCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new LineEndExtendCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecutePageDownKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new PageDownKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecutePageUpKeyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new PageUpKeyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteSelectAllCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new SelectAllCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteCopyCommand(Action next)
        {
            var commandHandlerService = _commandHandlerService;
            var nextCommandHandler = next;
            commandHandlerService.Execute((view, buffer) => new CopyCommandArgs(view, buffer), nextCommandHandler);
        }

        private void ExecuteCutCommand(Action next)
        {
            _commandHandlerService.Execute((view, buffer) => new CutCommandArgs(view, buffer), next);
        }

        private void ExecutePasteCommand(Action next)
        {
            _commandHandlerService.Execute((view, buffer) => new PasteCommandArgs(view, buffer), next);
        }
    }
}