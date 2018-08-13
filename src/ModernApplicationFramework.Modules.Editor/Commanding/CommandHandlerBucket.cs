using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Ui.Commanding;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    internal class CommandHandlerBucket
    {
        private readonly IReadOnlyList<Lazy<ITextEditCommand, ICommandHandlerMetadata>> _commandHandlers;
        private int _currentCommandHandlerIndex;

        public bool IsEmpty => _currentCommandHandlerIndex >= _commandHandlers.Count;

        public CommandHandlerBucket(IReadOnlyList<Lazy<ITextEditCommand, ICommandHandlerMetadata>> commandHandlers)
        {
            var lazyList = commandHandlers;
            _commandHandlers = lazyList ?? throw new ArgumentNullException(nameof(commandHandlers));
        }

        public Lazy<ITextEditCommand, ICommandHandlerMetadata> Peek()
        {
            if (!IsEmpty)
                return _commandHandlers[_currentCommandHandlerIndex];
            throw new InvalidOperationException($"{nameof(CommandHandlerBucket) as object} is empty.");
        }

        internal Lazy<ITextEditCommand, ICommandHandlerMetadata> Pop()
        {
            if (!IsEmpty)
                return _commandHandlers[_currentCommandHandlerIndex++];
            throw new InvalidOperationException($"{nameof(CommandHandlerBucket)} is empty.");
        }
    }
}