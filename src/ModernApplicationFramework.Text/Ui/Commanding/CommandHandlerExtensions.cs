using System;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public static class CommandHandlerExtensions
    {
        public static void ExecuteCommand<T>(this ICommandHandler commandHandler, T args, Action nextCommandHandler,
            CommandExecutionContext executionContext) where T : CommandArgs
        {
            if (commandHandler == null)
                throw new ArgumentNullException(nameof(commandHandler));
            if (nextCommandHandler == null)
                throw new ArgumentNullException(nameof(nextCommandHandler));
            ICommandHandler<T> commandHandler1;
            if ((commandHandler1 = commandHandler as ICommandHandler<T>) != null)
            {
                if (commandHandler1.ExecuteCommand(args, executionContext))
                    return;
                nextCommandHandler();
            }
            else
            {
                IChainedCommandHandler<T> chainedCommandHandler;
                if ((chainedCommandHandler = commandHandler as IChainedCommandHandler<T>) == null)
                    throw new ArgumentException($"Unsupported CommandHandler type: {commandHandler.GetType()}");
                chainedCommandHandler.ExecuteCommand(args, nextCommandHandler, executionContext);
            }
        }

        public static CommandState GetCommandState<T>(this ICommandHandler commandHandler, T args,
            Func<CommandState> nextCommandHandler) where T : CommandArgs
        {
            if (commandHandler == null)
                throw new ArgumentNullException(nameof(commandHandler));
            if (nextCommandHandler == null)
                throw new ArgumentNullException(nameof(nextCommandHandler));
            if (commandHandler is ICommandHandler<T> commandHandler1)
            {
                var commandState = commandHandler1.GetCommandState(args);
                if (commandState.IsUnspecified)
                    return nextCommandHandler();
                return commandState;
            }

            if (commandHandler is IChainedCommandHandler<T> chainedCommandHandler)
                return chainedCommandHandler.GetCommandState(args, nextCommandHandler);
            throw new ArgumentException($"Unsupported CommandHandler type: {commandHandler.GetType()}");
        }
    }
}