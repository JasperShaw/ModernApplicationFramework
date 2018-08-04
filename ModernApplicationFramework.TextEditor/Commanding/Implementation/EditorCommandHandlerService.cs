using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Commanding.Implementation
{
    internal class EditorCommandHandlerService : IEditorCommandHandlerService
    {
        private static readonly Action EmptyAction = () => { };
        private static readonly Func<CommandState> UnavalableCommandFunc = () => CommandState.Unavailable;
        private static readonly IReadOnlyList<Lazy<ICommandHandler, ICommandHandlerMetadata>> EmptyHandlerList =
            new List<Lazy<ICommandHandler, ICommandHandlerMetadata>>(0);

        private readonly IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> _commandHandlers;
        private readonly ITextView _textView;
        private readonly IUiThreadOperationExecutor _uiThreadOperationExecutor;
        private readonly StableContentTypeComparer _contentTypesComparer;
        private readonly IGuardedOperations _guardedOperations;
        private readonly ICommandingTextBufferResolver _bufferResolver;

        private readonly Dictionary<(Type, IContentType), IReadOnlyList<Lazy<ICommandHandler, ICommandHandlerMetadata>>>
            _commandHandlersByTypeAndContentType;

        public EditorCommandHandlerService(ITextView textView,
            IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> commandHandlers,
            IUiThreadOperationExecutor operationExecutor, StableContentTypeComparer contentTypesComparer,
            ICommandingTextBufferResolver bufferResolver, IGuardedOperations guardedOperations)
        {
            var lazies = commandHandlers;
            _commandHandlers = lazies ?? throw new ArgumentNullException(nameof(lazies));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _uiThreadOperationExecutor =
                operationExecutor ?? throw new ArgumentNullException(nameof(operationExecutor));
            _contentTypesComparer =
                contentTypesComparer ?? throw new ArgumentNullException(nameof(contentTypesComparer));
            _commandHandlersByTypeAndContentType =
                new Dictionary<ValueTuple<Type, IContentType>,
                    IReadOnlyList<Lazy<ICommandHandler, ICommandHandlerMetadata>>>();
            _bufferResolver = bufferResolver ?? throw new ArgumentNullException(nameof(bufferResolver));
            _guardedOperations = guardedOperations ?? throw new ArgumentNullException(nameof(guardedOperations));
        }

        public CommandState GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory,
            Func<CommandState> nextCommandHandler) where T : EditorCommandArgs
        {
            //if (!this._joinableTaskContext.IsOnMainThread)
            //    throw new InvalidOperationException(
            //        $"{nameof(GetCommandState)} method shoudl only be called on the UI thread.");
            if (IsReentrantCall())
            {
                if (nextCommandHandler == null)
                    return CommandState.Unavailable;
                return nextCommandHandler();
            }
            using (new ReentrancyGuard(_textView))
            {
                var func = nextCommandHandler ?? UnavalableCommandFunc;
                foreach (var valueTuple in GetOrderedBuffersAndCommandHandlers<T>().Reverse())
                {
                    T args = default;
                    var nextHandler = func;
                    var handler = valueTuple.Item2;
                    var obj = args;
                    obj = argsFactory(_textView, valueTuple.Item1);
                    args = obj;
                    if (args == null)
                        return func();
                    func = () => handler.GetCommandState(args, nextHandler);
                }
                return func();
            }
        }

        public void Execute<T>(Func<ITextView, ITextBuffer, T> argsFactory, Action nextCommandHandler)
            where T : EditorCommandArgs
        {
            //if (!this._joinableTaskContext.IsOnMainThread)
            //    throw new InvalidOperationException(
            //        $"{nameof(GetCommandState)} method shoudl only be called on the UI thread.");
            if (IsReentrantCall())
                nextCommandHandler?.Invoke();
            else
            {
                using (new ReentrancyGuard(_textView))
                {
                    CommandExecutionContext commandExecutionContext = null;
                    var handlerChain = nextCommandHandler ?? EmptyAction;
                    foreach (var valueTuple in GetOrderedBuffersAndCommandHandlers<T>().Reverse())
                    {
                        T args = default;
                        var nextHandler = handlerChain;
                        var handler = valueTuple.Item2;
                        var obj = args;
                        obj = argsFactory(_textView, valueTuple.Item1);
                        args = obj;
                        if (args == null)
                            handlerChain();
                        if (commandExecutionContext == null)
                            commandExecutionContext = CreateCommandExecutionContext();
                        handlerChain = () => _guardedOperations.CallExtensionPoint(handler, () => handler.ExecuteCommand(args, nextHandler, commandExecutionContext));
                    }

                    ExecuteCommandHandlerChain(commandExecutionContext, handlerChain, nextCommandHandler);
                }
            }
        }

        private CommandExecutionContext CreateCommandExecutionContext()
        {
            return new CommandExecutionContext(_uiThreadOperationExecutor.BeginExecute(null, "Wait for Command Execution", true, true));
        }

        private static void ExecuteCommandHandlerChain(CommandExecutionContext commandExecutionContext, Action handlerChain, Action nextCommandHandler)
        {
            try
            {
                handlerChain();
            }
            catch (OperationCanceledException)
            {
                if (nextCommandHandler == null)
                    return;
                nextCommandHandler();
            }
            catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is OperationCanceledException))
            {
                if (nextCommandHandler == null)
                    return;
                nextCommandHandler();
            }
            finally
            {
                commandExecutionContext?.OperationContext?.Dispose();
            }
        }

        internal IEnumerable<ValueTuple<ITextBuffer, ICommandHandler>> GetOrderedBuffersAndCommandHandlers<T>()
            where T : EditorCommandArgs
        {
            var buffers = (IReadOnlyList<ITextBuffer>) _bufferResolver.ResolveBuffersForCommand<T>().ToArray();
            if (buffers.Count != 0)
            {
                var handlerBuckets = new CommandHandlerBucket[buffers.Count];
                for (var index = 0; index < buffers.Count; ++index)
                    handlerBuckets[index] =
                        new CommandHandlerBucket(GetOrCreateOrderedHandlers<T>(buffers[index].ContentType,
                            _textView.Roles));
                while (true)
                {
                    var currentHandler = (Lazy<ICommandHandler, ICommandHandlerMetadata>) null;
                    var currentHandlerBufferIndex = 0;

                    for (var index = 0; index < handlerBuckets.Length; ++index)
                    {
                        if (!handlerBuckets[index].IsEmpty)
                        {
                            currentHandler = handlerBuckets[index].Peek();
                            currentHandlerBufferIndex = index;
                            break;
                        }
                    }

                    if (currentHandler != null)
                    {
                        var foundBetterHandler = false;
                        for (var i = 0; i < buffers.Count; ++i)
                        {
                            if (i != currentHandlerBufferIndex && !handlerBuckets[i].IsEmpty)
                            {
                                var lazy = handlerBuckets[i].Peek();
                                if (_contentTypesComparer.Compare(lazy.Metadata.ContentTypes, currentHandler.Metadata.ContentTypes) < 0)
                                {
                                    foundBetterHandler = true;
                                    handlerBuckets[i].Pop();
                                    yield return new ValueTuple<ITextBuffer, ICommandHandler>(buffers[i], lazy.Value);
                                    break;
                                }
                            }
                        }

                        if (foundBetterHandler)
                            continue;
                        yield return new ValueTuple<ITextBuffer, ICommandHandler>(buffers[currentHandlerBufferIndex], currentHandler.Value);
                        handlerBuckets[currentHandlerBufferIndex].Pop();
                    }
                    else
                        break;
                }
            }
        }

        private IReadOnlyList<Lazy<ICommandHandler, ICommandHandlerMetadata>> GetOrCreateOrderedHandlers<T>(
            IContentType contentType, ITextViewRoleSet textViewRoles) where T : EditorCommandArgs
        {
            var key = new ValueTuple<Type, IContentType>(typeof(T), contentType);
            if (!_commandHandlersByTypeAndContentType.TryGetValue(key, out var lazyList))
            {
                var source = (IList<Lazy<ICommandHandler, ICommandHandlerMetadata>>) null;
                foreach (var matchingCommandHandler in SelectMatchingCommandHandlers(_commandHandlers, contentType,
                    textViewRoles))
                {
                    var commandHandler = _guardedOperations.InstantiateExtension(this, matchingCommandHandler);
                    if (commandHandler is ICommandHandler<T> || commandHandler is IChainedCommandHandler<T>)
                    {
                        if (source == null)
                            source = new FrugalList<Lazy<ICommandHandler, ICommandHandlerMetadata>>();
                        source.Add(matchingCommandHandler);
                    }
                }

                if (source != null && source.Count > 1)
                    source = StableOrderer.Order(source).ToArray();
                lazyList = source?.ToArray() ?? EmptyHandlerList;
                _commandHandlersByTypeAndContentType[key] = lazyList;
            }

            return lazyList;
        }

        private static IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> SelectMatchingCommandHandlers(
            IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> commandHandlers, IContentType contentType,
            ITextViewRoleSet textViewRoles)
        {
            return commandHandlers.Where(commandHandler =>
                MatchesContentType(commandHandler.Metadata, contentType) &&
                MatchesTextViewRoles(commandHandler.Metadata, textViewRoles));
        }

        private static bool MatchesContentType(IContentTypeMetadata handlerMetadata, IContentType contentType)
        {
            return handlerMetadata.ContentTypes.Any(contentType.IsOfType);
        }

        private static bool MatchesTextViewRoles(ICommandHandlerMetadata handlerMetadata, ITextViewRoleSet roles)
        {
            if (handlerMetadata.TextViewRoles == null)
                return true;
            return handlerMetadata.TextViewRoles.Any(roles.Contains);
        }

        private bool IsReentrantCall()
        {
            return _textView.Properties.ContainsProperty(typeof(ReentrancyGuard));
        }

        private class ReentrancyGuard : IDisposable
        {
            private readonly IPropertyOwner _owner;

            public ReentrancyGuard(IPropertyOwner owner)
            {
                var propertyOwner = owner;
                _owner = propertyOwner ?? throw new ArgumentNullException(nameof(owner));
                _owner.Properties[typeof(ReentrancyGuard)] = this;
            }

            public void Dispose()
            {
                _owner.Properties.RemoveProperty(typeof(ReentrancyGuard));
            }
        }
    }
}