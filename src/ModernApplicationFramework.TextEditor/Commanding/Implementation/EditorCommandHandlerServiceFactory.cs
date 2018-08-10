using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Commanding.Implementation
{
    [Export(typeof(IEditorCommandHandlerServiceFactory))]
    internal class EditorCommandHandlerServiceFactory : IEditorCommandHandlerServiceFactory
    {
        private readonly IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> _commandHandlers;

        private readonly IList<Lazy<ICommandingTextBufferResolverProvider, IContentTypeMetadata>>
            _bufferResolverProviders;

        private readonly IUiThreadOperationExecutor _uiThreadOperationExecutor;
        private readonly IContentTypeRegistryService _contentTypeRegistryService;
        private readonly IGuardedOperations _guardedOperations;
        private readonly StableContentTypeComparer _contentTypeComparer;

        [ImportingConstructor]
        public EditorCommandHandlerServiceFactory(
            [ImportMany] IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> commandHandlers,
            [ImportMany] IEnumerable<Lazy<ICommandingTextBufferResolverProvider, IContentTypeMetadata>> bufferResolvers,
            IUiThreadOperationExecutor uiThreadOperationExecutor,
            //JoinableTaskContext joinableTaskContext, 
            IContentTypeRegistryService contentTypeRegistryService,
            IGuardedOperations guardedOperations)
        {
            _uiThreadOperationExecutor = uiThreadOperationExecutor;
            _contentTypeRegistryService = contentTypeRegistryService;
            _guardedOperations = guardedOperations;
            _commandHandlers = OrderCommandHandlers(commandHandlers);
            _contentTypeComparer = new StableContentTypeComparer(_contentTypeRegistryService);
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
                /*this._joinableTaskContext,*/ _contentTypeComparer, bufferResolver, _guardedOperations);
        }

        public IEditorCommandHandlerService GetService(ITextView textView, ITextBuffer subjectBuffer)
        {
            if (subjectBuffer == null)
                return GetService(textView);
            return new EditorCommandHandlerService(textView, _commandHandlers, 
                _uiThreadOperationExecutor, /*this._joinableTaskContext, */ _contentTypeComparer, 
                new SingleBufferResolver(subjectBuffer), _guardedOperations);
        }

        private IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> OrderCommandHandlers(
            IEnumerable<Lazy<ICommandHandler, ICommandHandlerMetadata>> commandHandlers)
        {
            var source = commandHandlers;
            var contentTypeComparer = _contentTypeComparer;
            return source.OrderBy(handler => handler.Metadata.ContentTypes, contentTypeComparer);
        }
    }
}