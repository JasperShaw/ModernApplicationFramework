using System;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public static class CommandHandlerExtensions
    {
        public static void ExecuteCommand<T>(this ITextEditCommand commandHandler, T args, Action nextCommandHandler,
            CommandExecutionContext executionContext) where T : CommandArgs
        {
            if (commandHandler == null)
                throw new ArgumentNullException(nameof(commandHandler));
            if (nextCommandHandler == null)
                throw new ArgumentNullException(nameof(nextCommandHandler));
            ITextEditCommand<T> commandHandler1;
            if ((commandHandler1 = commandHandler as ITextEditCommand<T>) != null)
            {
                if (commandHandler1.ExecuteCommand(args, executionContext))
                    return;
                nextCommandHandler();
            }
            else
            {
                IChainedTextEditCommand<T> chainedTextEditCommand;
                if ((chainedTextEditCommand = commandHandler as IChainedTextEditCommand<T>) == null)
                    throw new ArgumentException($"Unsupported CommandHandler type: {commandHandler.GetType()}");
                chainedTextEditCommand.ExecuteCommand(args, nextCommandHandler, executionContext);
            }
        }

        public static CommandState GetCommandState<T>(this ITextEditCommand commandHandler, T args,
            Func<CommandState> nextCommandHandler) where T : CommandArgs
        {
            if (commandHandler == null)
                throw new ArgumentNullException(nameof(commandHandler));
            if (nextCommandHandler == null)
                throw new ArgumentNullException(nameof(nextCommandHandler));
            if (commandHandler is ITextEditCommand<T> commandHandler1)
            {
                var commandState = commandHandler1.GetCommandState(args);
                if (commandState.IsUnspecified)
                    return nextCommandHandler();
                return commandState;
            }

            if (commandHandler is IChainedTextEditCommand<T> chainedCommandHandler)
                return chainedCommandHandler.GetCommandState(args, nextCommandHandler);
            throw new ArgumentException($"Unsupported CommandHandler type: {commandHandler.GetType()}");
        }
    }
}