using System;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.DragDrop;
using ModernApplicationFramework.Text.Ui.Operations;

namespace ModernApplicationFramework.Modules.Editor.DragDrop
{
    internal class DefaultTextDropHandler : DropHandlerBase
    {
        //TODO: Add Undo stuff

        //protected ITextUndoHistory _undoHistory;
        //protected ITextUndoTransaction _undoTransaction;

        public DefaultTextDropHandler(ITextView wpfTextView, /*ITextUndoHistory undoHistory,*/ IEditorOperations editorOperations)
            : base(wpfTextView, editorOperations)
        {
            if (wpfTextView == null)
                throw new ArgumentNullException(nameof(wpfTextView));
            //if (undoHistory == null)
            //    throw new ArgumentNullException(nameof(undoHistory));
            //if (editorOperations == null)
            //    throw new ArgumentNullException(nameof(editorOperations));
            //_undoHistory = undoHistory;
            //_undoTransaction = null;
        }

        protected override string ExtractText(DragDropInfo dragDropInfo)
        {
            return DataObjectManager.ExtractText(dragDropInfo.Data);
        }

        protected override void PerformPreEditActions(DragDropInfo dragDropInfo)
        {
            //_undoTransaction = _undoHistory.CreateTransaction();
            EditorOperations.AddBeforeTextBufferChangePrimitive();
        }

        protected override void PerformPostEditActions(DragDropInfo dragDropInfo, bool successfulEdit)
        {
            if (successfulEdit)
            {
                EditorOperations.AddAfterTextBufferChangePrimitive();
                //_undoTransaction.Complete();
            }
            //else
            //    _undoTransaction.Cancel();
            //_undoTransaction.Dispose();
            //_undoTransaction = null;
        }
    }
}