using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    [Export(typeof(IEditorCommandHandlerServiceFactory))]
    internal class EditorCommandHandlerServiceFactory : IEditorCommandHandlerServiceFactory
    {
        private readonly IList<Lazy<ICommandingTextBufferResolverProvider, IContentTypeMetadata>>
            _bufferResolverProviders;

        private readonly IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> _commandHandlers;
        private readonly StableContentTypeComparer _contentTypeComparer;
        private readonly IContentTypeRegistryService _contentTypeRegistryService;
        private readonly IGuardedOperations _guardedOperations;

        private readonly IUiThreadOperationExecutor _uiThreadOperationExecutor;
        private readonly JoinableTaskContext _joinableTaskContext;

        [ImportingConstructor]
        public EditorCommandHandlerServiceFactory(
            [ImportMany] IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> commandHandlers,
            [ImportMany] IEnumerable<Lazy<ICommandingTextBufferResolverProvider, IContentTypeMetadata>> bufferResolvers,
            IUiThreadOperationExecutor uiThreadOperationExecutor,
            JoinableTaskContext joinableTaskContext, 
            IContentTypeRegistryService contentTypeRegistryService,
            IGuardedOperations guardedOperations)
        {
            _uiThreadOperationExecutor = uiThreadOperationExecutor;
            _joinableTaskContext = joinableTaskContext;
            _contentTypeRegistryService = contentTypeRegistryService;
            _guardedOperations = guardedOperations;
            _contentTypeComparer = new StableContentTypeComparer(_contentTypeRegistryService);
            _commandHandlers = OrderCommandHandlers(commandHandlers);  
            if (!bufferResolvers.Any())
                throw new ImportCardinalityMismatchException();
            _bufferResolverProviders = bufferResolvers.ToList();
        }

        public IEditorCommandHandlerService GetService(ITextView textView)
        {
            var bufferResolverProvider = _guardedOperations.InvokeBestMatchingFactory(_bufferResolverProviders,
                textView.TextBuffer.ContentType, _contentTypeRegistryService, this);
            var bufferResolver = (ICommandingTextBufferResolver) null;
            _guardedOperations.CallExtensionPoint(
                () => bufferResolver = bufferResolverProvider.CreateResolver(textView));
            bufferResolver = bufferResolver ?? new DefaultBufferResolver(textView);
            return new EditorCommandHandlerService(textView, _commandHandlers, _uiThreadOperationExecutor,
                this._joinableTaskContext, _contentTypeComparer, bufferResolver, _guardedOperations);
        }

        public IEditorCommandHandlerService GetService(ITextView textView, ITextBuffer subjectBuffer)
        {
            if (subjectBuffer == null)
                return GetService(textView);
            return new EditorCommandHandlerService(textView, _commandHandlers,
                _uiThreadOperationExecutor, this._joinableTaskContext, _contentTypeComparer,
                new SingleBufferResolver(subjectBuffer), _guardedOperations);
        }

        private IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> OrderCommandHandlers(
            IEnumerable<Lazy<ITextEditCommand, ICommandHandlerMetadata>> commandHandlers)
        {
            var source = commandHandlers;
            var contentTypeComparer = _contentTypeComparer;
            return source.OrderBy(handler => handler.Metadata.ContentTypes, contentTypeComparer);
        }
    }
}