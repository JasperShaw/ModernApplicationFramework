using System.Globalization;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal class BraceCompletionDefaultSession : IBraceCompletionSession
    {
        private readonly IBraceCompletionContext _context;
        //private ITextBufferUndoManagerProvider _undoManager;
        //private ITextUndoHistory _undoHistory;
        private readonly IEditorOperations _editorOperations;

        public ITrackingPoint OpeningPoint { get; private set; }

        public ITrackingPoint ClosingPoint { get; private set; }

        public ITextView TextView { get; }

        public ITextBuffer SubjectBuffer { get; }

        public char OpeningBrace { get; }

        public char ClosingBrace { get; }

        private SnapshotPoint? CaretPosition => GetCaretPoint(SubjectBuffer);

        private bool HasForwardTyping
        {
            get
            {
                var point = ClosingPoint.GetPoint(SubjectBuffer.CurrentSnapshot);
                if (point.Position > 0)
                {
                    var caretPosition = CaretPosition;
                    if (caretPosition.HasValue && !HasNoForwardTyping(caretPosition.Value, point.Subtract(1)))
                        return true;
                }
                return false;
            }
        }

        public BraceCompletionDefaultSession(ITextView textView, ITextBuffer subjectBuffer, SnapshotPoint openingPoint, char openingBrace, char closingBrace, /*ITextBufferUndoManagerProvider undoManager,*/ IEditorOperationsFactoryService editorOperationsFactoryService)
            : this(textView, subjectBuffer, openingPoint, openingBrace, closingBrace, /*undoManager,*/ editorOperationsFactoryService, null)
        {
        }

        public BraceCompletionDefaultSession(ITextView textView, ITextBuffer subjectBuffer, SnapshotPoint openingPoint, char openingBrace, char closingBrace, /*ITextBufferUndoManagerProvider undoManager,*/ IEditorOperationsFactoryService editorOperationsFactoryService, IBraceCompletionContext context)
        {
            TextView = textView;
            SubjectBuffer = subjectBuffer;
            OpeningBrace = openingBrace;
            ClosingBrace = closingBrace;
            ClosingPoint = SubjectBuffer.CurrentSnapshot.CreateTrackingPoint(openingPoint.Position, PointTrackingMode.Positive);
            _context = context;
            //_undoManager = undoManager;
            //_undoHistory = undoManager.GetTextBufferUndoManager(_textView.TextBuffer).TextBufferUndoHistory;
            _editorOperations = editorOperationsFactoryService.GetEditorOperations(TextView);
        }

        public void Start()
        {
            var bufferPosition = TextView.Caret.Position.BufferPosition;
            var trackingPoint =
                bufferPosition.Snapshot.CreateTrackingPoint(bufferPosition.Position, PointTrackingMode.Negative);
            var snapshot = SubjectBuffer.CurrentSnapshot;
            var point1 = ClosingPoint.GetPoint(snapshot);
            if (point1.Position < 1)
                EndSession();
            else
            {
                var snapshotPoint = point1.Subtract(1);
                if (snapshotPoint.GetChar() != OpeningBrace)
                    EndSession();
                else
                {
                    OpeningPoint =
                        SubjectBuffer.CurrentSnapshot.CreateTrackingPoint(snapshotPoint, PointTrackingMode.Positive);

                    //TODO: undo
                    //using (ITextUndoTransaction undoTransaction = this.CreateUndoTransaction())
                    //{
                    using (var edit = SubjectBuffer.CreateEdit())
                    {
                        edit.Insert(point1, ClosingBrace.ToString(CultureInfo.CurrentCulture));
                        if (edit.HasFailedChanges)
                        {
                            edit.Cancel();
                            //undoTransaction.Cancel();
                            return;
                        }

                        snapshot = edit.Apply();
                    }

                    var point2 = trackingPoint.GetPoint(TextView.TextSnapshot);
                    ClosingPoint =
                        SubjectBuffer.CurrentSnapshot.CreateTrackingPoint(ClosingPoint.GetPoint(snapshot),
                            PointTrackingMode.Negative);
                    TextView.Caret.MoveTo(point2);
                    _context?.Start(this);
                    //undoTransaction.Complete();
                    //}
                }
            }
        }

        public void Finish()
        {
            _context?.Finish(this);
        }

        public void PreOverType(out bool handledCommand)
        {
            handledCommand = false;
            if (HasForwardTyping || _context != null && !_context.AllowOverType(this))
                return;
            var caretPosition = CaretPosition;
            var point = ClosingPoint.GetPoint(SubjectBuffer.CurrentSnapshot);
            if (!caretPosition.HasValue || (caretPosition.Value.Position >= point.Position || point.Position <= 0))
                return;
            //TODO: undo
            //using (ITextUndoTransaction undoTransaction = this.CreateUndoTransaction())
            {
                _editorOperations.AddBeforeTextBufferChangePrimitive();
                var snapshotSpan = new SnapshotSpan(caretPosition.Value, point.Subtract(1));
                using (var edit = SubjectBuffer.CreateEdit())
                {
                    edit.Delete(snapshotSpan);
                    if (edit.HasFailedChanges)
                    {
                        edit.Cancel();
                        //undoTransaction.Cancel();
                    }
                    else
                    {
                        handledCommand = true;
                        edit.Apply();
                        MoveCaretToClosingPoint();
                        _editorOperations.AddAfterTextBufferChangePrimitive();
                        //undoTransaction.Complete();
                    }
                }
            }
        }

        public void PostOverType()
        {
        }

        public void PreTab(out bool handledCommand)
        {
            handledCommand = false;
            if (HasForwardTyping)
                return;
            handledCommand = true;
            //TODO: undo
            //using (ITextUndoTransaction undoTransaction = this.CreateUndoTransaction())
            {
                _editorOperations.AddBeforeTextBufferChangePrimitive();
                MoveCaretToClosingPoint();
                _editorOperations.AddAfterTextBufferChangePrimitive();
               // undoTransaction.Complete();
            }
        }

        public void PostTab()
        {
        }

        public void PreBackspace(out bool handledCommand)
        {
            handledCommand = false;
            var caretPosition = CaretPosition;
            var currentSnapshot = SubjectBuffer.CurrentSnapshot;
            if (!caretPosition.HasValue || caretPosition.Value.Position <= 0 || (caretPosition.Value.Position - 1 != OpeningPoint.GetPoint(currentSnapshot).Position || HasForwardTyping))
                return;

            //TODO: undo
            //using (ITextUndoTransaction undoTransaction = this.CreateUndoTransaction())
            //{
                using (var edit = SubjectBuffer.CreateEdit())
                {
                    var snapshotSpan = new SnapshotSpan(OpeningPoint.GetPoint(currentSnapshot), ClosingPoint.GetPoint(currentSnapshot));
                    edit.Delete(snapshotSpan);
                    if (edit.HasFailedChanges)
                    {
                        edit.Cancel();
                        //undoTransaction.Cancel();
                    }
                    else
                    {
                        handledCommand = true;
                        edit.Apply();
                        //undoTransaction.Complete();
                        EndSession();
                    }
                }
            //}
        }

        public void PostBackspace()
        {
        }

        public void PreDelete(out bool handledCommand)
        {
            handledCommand = false;
        }

        public void PostDelete()
        {
        }

        public void PreReturn(out bool handledCommand)
        {
            handledCommand = false;
        }

        public void PostReturn()
        {
            if (_context == null || !CaretPosition.HasValue)
                return;
            var point = ClosingPoint.GetPoint(SubjectBuffer.CurrentSnapshot);
            if (point.Position <= 0 || !HasNoForwardTyping(CaretPosition.Value, point.Subtract(1)))
                return;
            _context.OnReturn(this);
        }

        private void EndSession()
        {
            OpeningPoint = null;
            ClosingPoint = null;
        }

        private SnapshotPoint? GetCaretPoint(ITextBuffer buffer)
        {
            return TextView.Caret.Position.Point.GetPoint(buffer, PositionAffinity.Predecessor);
        }

        private static bool HasNoForwardTyping(SnapshotPoint caretPoint, SnapshotPoint endPoint)
        {
            if (caretPoint.Snapshot == endPoint.Snapshot)
            {
                if (caretPoint == endPoint)
                    return true;
                if (caretPoint.Position < endPoint.Position)
                    return string.IsNullOrWhiteSpace(new SnapshotSpan(caretPoint, endPoint).GetText());
            }
            return false;
        }

        private void MoveCaretToClosingPoint()
        {
            var buffer = TextView.BufferGraph.MapUpToBuffer(ClosingPoint.GetPoint(SubjectBuffer.CurrentSnapshot), PointTrackingMode.Negative, PositionAffinity.Predecessor, TextView.TextBuffer);
            if (!buffer.HasValue)
                return;
            TextView.Caret.MoveTo(buffer.Value);
        }
    }
}
