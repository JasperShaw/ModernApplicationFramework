using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor.Commanding.Implementation
{
    internal class CommandHandlerBucket
    {
        private int _currentCommandHandlerIndex;
        private readonly IReadOnlyList<Lazy<ICommandHandler, ICommandHandlerMetadata>> _commandHandlers;

        public CommandHandlerBucket(IReadOnlyList<Lazy<ICommandHandler, ICommandHandlerMetadata>> commandHandlers)
        {
            var lazyList = commandHandlers;
            _commandHandlers = lazyList ?? throw new ArgumentNullException(nameof(commandHandlers));
        }

        public bool IsEmpty => _currentCommandHandlerIndex >= _commandHandlers.Count;

        public Lazy<ICommandHandler, ICommandHandlerMetadata> Peek()
        {
            if (!IsEmpty)
                return _commandHandlers[_currentCommandHandlerIndex];
            throw new InvalidOperationException($"{nameof(CommandHandlerBucket) as object} is empty.");
        }

        internal Lazy<ICommandHandler, ICommandHandlerMetadata> Pop()
        {
            if (!IsEmpty)
                return _commandHandlers[_currentCommandHandlerIndex++];
            throw new InvalidOperationException($"{nameof(CommandHandlerBucket)} is empty.");
        }
    }
}