using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    internal class EditorCommandHandlerService : IEditorCommandHandlerService
    {
        private static readonly Action EmptyAction = () => { };

        private static readonly IReadOnlyList<Lazy<ITextEditCommand, ICommandHandlerMetadata>> EmptyHandlerList =
            new List<Lazy<ITextEditCommand, ICommandHandlerMetadata>>(0);

        private static readonly Func<CommandState> UnavalableCommandFunc = () => CommandState.Unavailable;
        private readonly ICommandingTextBufferResolver _bufferResolver;

        private readonly IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> _commandHandlers;

        private readonly Dictionary<(Type, IContentType), IReadOnlyList<Lazy<ITextEditCommand, ICommandHandlerMetadata>>>
            _commandHandlersByTypeAndContentType;

        private readonly IComparer<IEnumerable<string>> _contentTypesComparer;
        private readonly IGuardedOperations _guardedOperations;
        private readonly ITextView _textView;
        private readonly IUiThreadOperationExecutor _uiThreadOperationExecutor;
        private readonly JoinableTaskContext _joinableTaskContext;

        public EditorCommandHandlerService(ITextView textView, IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> commandHandlers, 
            IUiThreadOperationExecutor operationExecutor, JoinableTaskContext joinableTaskContext, 
            IComparer<IEnumerable<string>> contentTypesComparer, ICommandingTextBufferResolver bufferResolver, 
            IGuardedOperations guardedOperations)
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
                    IReadOnlyList<Lazy<ITextEditCommand, ICommandHandlerMetadata>>>();
            _joinableTaskContext = joinableTaskContext ?? throw new ArgumentNullException(nameof(bufferResolver));
            _bufferResolver = bufferResolver ?? throw new ArgumentNullException(nameof(bufferResolver));
            _guardedOperations = guardedOperations ?? throw new ArgumentNullException(nameof(guardedOperations));
        }

        public void Execute<T>(Func<ITextView, ITextBuffer, T> argsFactory, Action nextCommandHandler)
            where T : EditorCommandArgs
        {
            if (!_joinableTaskContext.IsOnMainThread)
                throw new InvalidOperationException(
                    $"{nameof(GetCommandState)} method shoudl only be called on the UI thread.");
            if (IsReentrantCall())
                nextCommandHandler?.Invoke();
            else
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
                        handlerChain = () => _guardedOperations.CallExtensionPoint(handler,
                            () => handler.ExecuteCommand(args, nextHandler, commandExecutionContext));
                    }

                    ExecuteCommandHandlerChain(commandExecutionContext, handlerChain, nextCommandHandler);
                }
        }

        public CommandState GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory,
            Func<CommandState> nextCommandHandler) where T : EditorCommandArgs
        {
            if (!_joinableTaskContext.IsOnMainThread)
                throw new InvalidOperationException(
                    $"{nameof(GetCommandState)} method shoudl only be called on the UI thread.");
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

        internal IEnumerable<ValueTuple<ITextBuffer, ITextEditCommand>> GetOrderedBuffersAndCommandHandlers<T>()
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
                    var currentHandler = (Lazy<ITextEditCommand, ICommandHandlerMetadata>) null;
                    var currentHandlerBufferIndex = 0;

                    for (var index = 0; index < handlerBuckets.Length; ++index)
                        if (!handlerBuckets[index].IsEmpty)
                        {
                            currentHandler = handlerBuckets[index].Peek();
                            currentHandlerBufferIndex = index;
                            break;
                        }

                    if (currentHandler != null)
                    {
                        var foundBetterHandler = false;
                        for (var i = 0; i < buffers.Count; ++i)
                            if (i != currentHandlerBufferIndex && !handlerBuckets[i].IsEmpty)
                            {
                                var lazy = handlerBuckets[i].Peek();
                                if (_contentTypesComparer.Compare(lazy.Metadata.ContentTypes,
                                        currentHandler.Metadata.ContentTypes) < 0)
                                {
                                    foundBetterHandler = true;
                                    handlerBuckets[i].Pop();
                                    yield return new ValueTuple<ITextBuffer, ITextEditCommand>(buffers[i], lazy.Value);
                                    break;
                                }
                            }

                        if (foundBetterHandler)
                            continue;
                        yield return new ValueTuple<ITextBuffer, ITextEditCommand>(buffers[currentHandlerBufferIndex],
                            currentHandler.Value);
                        handlerBuckets[currentHandlerBufferIndex].Pop();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static void ExecuteCommandHandlerChain(CommandExecutionContext commandExecutionContext,
            Action handlerChain, Action nextCommandHandler)
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

        private static IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> SelectMatchingCommandHandlers(
            IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> commandHandlers, IContentType contentType,
            ITextViewRoleSet textViewRoles)
        {
            foreach (var commandHandler in commandHandlers)
            {
                if (MatchesContentType(commandHandler.Metadata, contentType) && MatchesTextViewRoles(commandHandler.Metadata, textViewRoles))
                    yield return commandHandler;
            }
        }

        private CommandExecutionContext CreateCommandExecutionContext()
        {
            return new CommandExecutionContext(
                _uiThreadOperationExecutor.BeginExecute(null, "Wait for Command Execution", true, true));
        }

        private IReadOnlyList<Lazy<ITextEditCommand, ICommandHandlerMetadata>> GetOrCreateOrderedHandlers<T>(
            IContentType contentType, ITextViewRoleSet textViewRoles) where T : EditorCommandArgs
        {
            var key = new ValueTuple<Type, IContentType>(typeof(T), contentType);
            if (!_commandHandlersByTypeAndContentType.TryGetValue(key, out var lazyList))
            {
                var source = (IList<Lazy<ITextEditCommand, ICommandHandlerMetadata>>) null;


                var l = SelectMatchingCommandHandlers(_commandHandlers, contentType,
                        textViewRoles);


                foreach (var matchingCommandHandler in SelectMatchingCommandHandlers(_commandHandlers, contentType,
                    textViewRoles))
                {
                    var commandHandler = _guardedOperations.InstantiateExtension(this, matchingCommandHandler);
                    if (commandHandler is ITextEditCommand<T> || commandHandler is IChainedTextEditCommand<T>)
                    {
                        if (source == null)
                            source = new FrugalList<Lazy<ITextEditCommand, ICommandHandlerMetadata>>();
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