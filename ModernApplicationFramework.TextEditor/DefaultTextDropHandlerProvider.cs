using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.DragDrop;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IDropHandlerProvider))]
    [DropFormat("Text")]
    [DropFormat("System.String")]
    [DropFormat("UnicodeText")]
    [DropFormat("OEMText")]
    [DropFormat("HTML Format")]
    [DropFormat("CSV")]
    [Name("DefaultTextDropHandler")]
    [Order]
    internal class DefaultTextDropHandlerProvider : IDropHandlerProvider
    {
        //TODO: Add undo stuff

        //[Import]
        //internal ITextUndoHistoryRegistry UndoHistoryRegistry { get; set; }

        [Import]
        internal IEditorOperationsFactoryService EditorOperationsFactoryService { get; set; }

        public IDropHandler GetAssociatedDropHandler(ITextView wpfTextView)
        {
            if (wpfTextView == null)
                throw new ArgumentNullException(nameof(wpfTextView));
            return wpfTextView.Properties.GetOrCreateSingletonProperty(() =>
                new DefaultTextDropHandler(
                    wpfTextView, /*UndoHistoryRegistry.RegisterHistory((object)wpfTextView.TextBuffer),*/
                    EditorOperationsFactoryService.GetEditorOperations(wpfTextView)));
        }
    }
}