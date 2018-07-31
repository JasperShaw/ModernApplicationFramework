using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IDropHandlerProvider))]
    [DropFormat("MSDEVColumnSelect")]
    [Name("DefaultBoxTextDropHandler")]
    [Order(Before = "DefaultTextDropHandler")]
    internal class DefaultBoxTextDropHandlerProvider : IDropHandlerProvider
    {
        //[Import]
        //internal ITextUndoHistoryRegistry UndoHistoryRegistry { get; set; }

        [Import]
        internal IEditorOperationsFactoryService EditorOperationsFactoryService { get; set; }

        public IDropHandler GetAssociatedDropHandler(ITextView wpfTextView)
        {
            if (wpfTextView == null)
                throw new ArgumentNullException(nameof(wpfTextView));
            return wpfTextView.Properties.GetOrCreateSingletonProperty(() => new DefaultBoxTextDropHandler(wpfTextView,
                //UndoHistoryRegistry.RegisterHistory((object) wpfTextView.TextBuffer),
                EditorOperationsFactoryService.GetEditorOperations(wpfTextView)));
        }
    }
}