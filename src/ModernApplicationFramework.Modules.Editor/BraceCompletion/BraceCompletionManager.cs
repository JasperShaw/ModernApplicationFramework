using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal class BraceCompletionManager : IBraceCompletionManager
    {
        private readonly ITextView _textView;
        private readonly IBraceCompletionStack _stack;
        private IBraceCompletionAggregatorFactory _sessionFactory;
        private readonly IGuardedOperations _guardedOperations;
        private readonly IBraceCompletionAggregator _sessionAggregator;
        private bool _braceCompletionEnabled;
        private IBraceCompletionSession _waitingSession;
        private IBraceCompletionSession _postSession;
        private SnapshotPoint? _waitingSessionOpeningPoint;

        public bool Enabled => _braceCompletionEnabled;

        public bool HasActiveSessions => _stack.TopSession != null;

        public string OpeningBraces => _sessionAggregator.OpeningBraces;

        public string ClosingBraces => _sessionAggregator.ClosingBraces;

        private bool HasSelection => !_textView.Selection.IsEmpty;

        private bool HasForwardTypingOnLine
        {
            get
            {
                var bufferPosition = _textView.Caret.Position.BufferPosition;
                var end = _textView.Caret.ContainingTextViewLine.End;
                if (bufferPosition != end)
                {
                    if (_stack.TopSession != null)
                    {
                        ITrackingPoint closingPoint = null;
                        var session = _stack.TopSession;
                        _guardedOperations.CallExtensionPoint(() =>
                        {
                            if (session.OpeningPoint == null || session.ClosingPoint == null || session.OpeningPoint.TextBuffer != session.ClosingPoint.TextBuffer)
                                return;
                            closingPoint = session.ClosingPoint;
                        });
                        if (closingPoint != null)
                        {
                            SnapshotPoint? nullable = closingPoint.GetPoint(closingPoint.TextBuffer.CurrentSnapshot);
                            if (nullable.HasValue && _stack.TopSession.SubjectBuffer != _textView.TextBuffer)
                                nullable = _textView.BufferGraph.MapUpToBuffer(nullable.Value, closingPoint.TrackingMode, PositionAffinity.Predecessor, _textView.TextBuffer);
                            if (nullable.HasValue && nullable.Value.Position <= end && nullable.Value.Position > 0)
                                end = nullable.Value.Subtract(1);
                        }
                    }
                    if (bufferPosition == end)
                        return false;
                    if (!(bufferPosition < end))
                        return true;
                    var snapshotSpan = new SnapshotSpan(bufferPosition, end);
                    if (!snapshotSpan.IsEmpty)
                        return !string.IsNullOrWhiteSpace(snapshotSpan.GetText());
                }
                return false;
            }
        }


        internal BraceCompletionManager(ITextView textView, IBraceCompletionStack stack, IBraceCompletionAggregatorFactory sessionFactory, IGuardedOperations guardedOperations)
        {
            _textView = textView;
            _stack = stack;
            _sessionFactory = sessionFactory;
            _guardedOperations = guardedOperations;
            _sessionAggregator = sessionFactory.CreateAggregator();
            GetOptions();
            RegisterEvents();
        }

        public void PreTypeChar(char character, out bool handledCommand)
        {
            var handled = false;
            var hasSelection = HasSelection;
            if (_stack.TopSession != null && !hasSelection)
            {
                var session = _stack.TopSession;
                _guardedOperations.CallExtensionPoint(() =>
                {
                    if (!session.ClosingBrace.Equals(character) || !IsCaretOnBuffer(session.SubjectBuffer))
                        return;
                    session.PreOverType(out handled);
                    if (handled)
                        return;
                    _postSession = session;
                });
            }
            handledCommand = handled;
            if (_postSession != null || handled || !Enabled || hasSelection || _sessionAggregator.OpeningBraces.IndexOf(character) <= -1 || HasForwardTypingOnLine)
                return;
            var insertionPoint = _textView.Caret.Position.Point.GetInsertionPoint(b => _sessionAggregator.IsSupportedContentType(b.ContentType, character));
            if (!insertionPoint.HasValue)
                return;
            if (!_sessionAggregator.TryCreateSession(_textView, insertionPoint.Value, character, out var session1))
                return;
            _waitingSession = session1;
            _waitingSessionOpeningPoint = insertionPoint;
        }

        public void PostTypeChar(char character)
        {
            if (_waitingSession != null)
            {
                if (ValidateStart(_waitingSessionOpeningPoint, character))
                    _stack.PushSession(_waitingSession);
                _waitingSession = null;
                _waitingSessionOpeningPoint = new SnapshotPoint?();
            }
            else
            {
                if (_postSession == null)
                    return;
                _guardedOperations.CallExtensionPoint(() =>
                {
                    if (!_postSession.ClosingBrace.Equals(character))
                        return;
                    _postSession.PostOverType();
                });
                _postSession = null;
            }
        }

        public void PreTab(out bool handledCommand)
        {
            var handled = false;
            if (_stack.TopSession != null && !HasSelection)
            {
                var session = _stack.TopSession;
                _guardedOperations.CallExtensionPoint(() =>
                {
                    if (!IsSingleLine(session.OpeningPoint, session.ClosingPoint))
                        return;
                    session.PreTab(out handled);
                    if (handled)
                        return;
                    _postSession = session;
                });
            }
            handledCommand = handled;
        }

        public void PostTab()
        {
            if (_postSession == null)
                return;
            _guardedOperations.CallExtensionPoint(() => _postSession.PostTab());
            _postSession = null;
        }

        public void PreBackspace(out bool handledCommand)
        {
            var handled = false;
            if (_stack.TopSession != null && !HasSelection)
            {
                var session = _stack.TopSession;
                _guardedOperations.CallExtensionPoint(() =>
                {
                    if (session.OpeningPoint == null || session.ClosingPoint == null)
                        return;
                    session.PreBackspace(out handled);
                    if (handled)
                        return;
                    _postSession = session;
                });
            }
            handledCommand = handled;
        }

        public void PostBackspace()
        {
            if (_postSession == null)
                return;
            _guardedOperations.CallExtensionPoint(() => _postSession.PostBackspace());
            _postSession = null;
        }

        public void PreDelete(out bool handledCommand)
        {
            var handled = false;
            if (_stack.TopSession != null && !HasSelection)
            {
                var session = _stack.TopSession;
                _guardedOperations.CallExtensionPoint(() =>
                {
                    if (session.OpeningPoint == null || session.ClosingPoint == null)
                        return;
                    session.PreDelete(out handled);
                    if (handled)
                        return;
                    _postSession = session;
                });
            }
            handledCommand = handled;
        }

        public void PostDelete()
        {
            if (_postSession == null)
                return;
            _guardedOperations.CallExtensionPoint(() => _postSession.PostDelete());
            _postSession = null;
        }

        public void PreReturn(out bool handledCommand)
        {
            var handled = false;
            if (_stack.TopSession != null && !HasSelection)
            {
                var session = _stack.TopSession;
                _guardedOperations.CallExtensionPoint(() =>
                {
                    if (!IsSingleLine(session.OpeningPoint, session.ClosingPoint))
                        return;
                    session.PreReturn(out handled);
                    if (handled)
                        return;
                    _postSession = session;
                });
            }
            handledCommand = handled;
        }

        public void PostReturn()
        {
            if (_postSession == null)
                return;
            _guardedOperations.CallExtensionPoint(() => _postSession.PostReturn());
            _postSession = null;
        }

        private void RegisterEvents()
        {
            _textView.Closed += TextView_Closed;
            _textView.Options.OptionChanged += Options_OptionChanged;
        }

        private void UnregisterEvents()
        {
            _textView.Closed -= TextView_Closed;
            _textView.Options.OptionChanged -= Options_OptionChanged;
        }

        private bool ValidateStart(SnapshotPoint? openingPoint, char openingChar)
        {
            if (openingPoint.HasValue)
            {
                var point = _textView.Caret.Position.Point.GetPoint(openingPoint.Value.Snapshot.TextBuffer, PositionAffinity.Predecessor);
                if (point.HasValue && point.Value.Position > 0)
                    return point.Value.Subtract(1).GetChar() == openingChar;
            }
            return false;
        }

        private void GetOptions()
        {
            var num = _braceCompletionEnabled ? 1 : 0;
            _braceCompletionEnabled =
                _textView.Options.GetOptionValue(DefaultTextViewOptions.BraceCompletionEnabledOptionId);
            if (num == 0 || _braceCompletionEnabled)
                return;
            _waitingSession = null;
            _postSession = null;
            _stack.Clear();
        }

        private bool IsCaretOnBuffer(ITextBuffer buffer)
        {
            return _textView.Caret.Position.Point.GetPoint(buffer, PositionAffinity.Successor).HasValue;
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            UnregisterEvents();
        }

        private void Options_OptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            GetOptions();
        }

        private static bool IsSingleLine(ITrackingPoint openingPoint, ITrackingPoint closingPoint)
        {
            if (openingPoint == null || closingPoint == null)
                return false;
            var currentSnapshot = openingPoint.TextBuffer.CurrentSnapshot;
            var snapshotPoint = openingPoint.GetPoint(currentSnapshot);
            snapshotPoint = snapshotPoint.GetContainingLine().End;
            return snapshotPoint.Position >= closingPoint.GetPoint(currentSnapshot).Position;
        }
    }
}
