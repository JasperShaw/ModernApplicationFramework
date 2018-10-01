using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    [Export(typeof(IBraceCompletionAggregatorFactory))]
    internal class BraceCompletionAggregatorFactory : IBraceCompletionAggregatorFactory
    {
        internal IEnumerable<Lazy<IBraceCompletionSessionProvider, IBraceCompletionMetadata>> SessionProviders { get; private set; }

        internal IEnumerable<Lazy<IBraceCompletionContextProvider, IBraceCompletionMetadata>> ContextProviders { get; private set; }

        internal IEnumerable<Lazy<IBraceCompletionDefaultProvider, IBraceCompletionMetadata>> DefaultProviders { get; private set; }

        internal IContentTypeRegistryService ContentTypeRegistryService { get; private set; }

        //internal ITextBufferUndoManagerProvider UndoManager { get; private set; }

        internal IEditorOperationsFactoryService EditorOperationsFactoryService { get; private set; }

        internal IGuardedOperations GuardedOperations { get; private set; }

        [ImportingConstructor]
        public BraceCompletionAggregatorFactory(
            [ImportMany(typeof(IBraceCompletionSessionProvider))] IEnumerable<Lazy<IBraceCompletionSessionProvider,
            IBraceCompletionMetadata>> sessionProviders,
            [ImportMany(typeof(IBraceCompletionContextProvider))] IEnumerable<Lazy<IBraceCompletionContextProvider,
                IBraceCompletionMetadata>> contextProviders,
            [ImportMany(typeof(IBraceCompletionDefaultProvider))] IEnumerable<Lazy<IBraceCompletionDefaultProvider,
                IBraceCompletionMetadata>> defaultProviders,
            IContentTypeRegistryService contentTypeRegistryService,
            //TODO: Undo
            //ITextBufferUndoManagerProvider undoManager,
            IEditorOperationsFactoryService editorOperationsFactoryService,
            IGuardedOperations guardedOperations)
        {
            SessionProviders = sessionProviders;
            ContextProviders = contextProviders;
            DefaultProviders = defaultProviders;
            ContentTypeRegistryService = contentTypeRegistryService;
            //TODO: undo
            //UndoManager = undoManager;
            EditorOperationsFactoryService = editorOperationsFactoryService;
            GuardedOperations = guardedOperations;
        }

        public IBraceCompletionAggregator CreateAggregator()
        {
            return new BraceCompletionAggregator(this);
        }

        public IEnumerable<string> ContentTypes
        {
            get
            {
                return DefaultProviders.SelectMany(export => export.Metadata.ContentTypes)
                    .Concat(ContextProviders.SelectMany(export => export.Metadata.ContentTypes))
                    .Concat(SessionProviders.SelectMany(export => export.Metadata.ContentTypes));
            }
        }
    }
}
