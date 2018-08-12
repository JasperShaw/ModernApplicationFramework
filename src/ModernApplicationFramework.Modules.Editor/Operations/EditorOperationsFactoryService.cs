using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.TextEditor;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    //TODO: Reactivate Stuff

    [Export(typeof(IEditorOperationsFactoryService))]
    internal sealed class EditorOperationsFactoryService : IEditorOperationsFactoryService
    {
        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorFactory { get; set; }

        //[Import(AllowDefault = true)]
        //internal IWaitIndicator WaitIndicator { get; set; }

        //TODO: Add Search Stuff

        //[Import]
        //internal ITextSearchService TextSearchService { get; set; }

        //TODO: Add undo stuff

        //[Import]
        //internal ITextUndoHistoryRegistry UndoHistoryRegistry { get; set; }

        //[Import]
        //internal ITextBufferUndoManagerProvider TextBufferUndoManagerProvider { get; set; }

        [Import]
        internal IEditorPrimitivesFactoryService EditorPrimitivesProvider { get; set; }

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsProvider { get; set; }

        [Import]
        internal IRtfBuilderService RtfBuilderService { get; set; }

        [Import]
        internal ISmartIndentationService SmartIndentationService { get; set; }

        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [Import(AllowDefault = true)]
        internal IOutliningManagerService OutliningManagerService { get; set; }

        public IEditorOperations GetEditorOperations(ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (textView.Properties.TryGetProperty(typeof(EditorOperationsFactoryService),
                out IEditorOperations property))
                return property;
            property = new EditorOperations(textView, this);
            textView.Properties.AddProperty(typeof(EditorOperationsFactoryService), property);
            return property;
        }
    }
}