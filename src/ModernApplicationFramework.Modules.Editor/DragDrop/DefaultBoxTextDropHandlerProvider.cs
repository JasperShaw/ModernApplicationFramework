using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.DragDrop;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.DragDrop
{
    [Export(typeof(IDropHandlerProvider))]
    [DropFormat("ColumnSelect")]
    [Name("DefaultBoxTextDropHandler")]
    [Order(Before = "DefaultTextDropHandler")]
    internal class DefaultBoxTextDropHandlerProvider : IDropHandlerProvider
    {
        //TODO: Undo
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