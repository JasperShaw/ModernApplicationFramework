using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal class BraceCompletionStack : IBraceCompletionStack
    {
        private readonly Stack<IBraceCompletionSession> _stack;
        private ITextView _textView;
        private ITextBuffer _currentSubjectBuffer;
        private readonly IBraceCompletionAdornmentServiceFactory _adornmentServiceFactory;
        private IBraceCompletionAdornmentService _adornmentService;
        private readonly IGuardedOperations _guardedOperations;

        public BraceCompletionStack(ITextView textView, IBraceCompletionAdornmentServiceFactory adornmentFactory, IGuardedOperations guardedOperations)
        {
            _adornmentServiceFactory = adornmentFactory;
            _stack = new Stack<IBraceCompletionSession>();
            _textView = textView;
            _guardedOperations = guardedOperations;
            RegisterEvents();
        }

        public IBraceCompletionSession TopSession
        {
            get
            {
                if (_stack.Count <= 0)
                    return null;
                return _stack.Peek();
            }
        }

        public void PushSession(IBraceCompletionSession session)
        {
            ITextView view = null;
            ITextBuffer buffer = null;
            _guardedOperations.CallExtensionPoint(() =>
            {
                view = session.TextView;
                buffer = session.SubjectBuffer;
            });
            if (view == null || buffer == null)
                return;
            SetCurrentBuffer(buffer);
            bool validStart = false;
            _guardedOperations.CallExtensionPoint(() =>
            {
                session.Start();
                validStart = session.OpeningPoint != null && session.ClosingPoint != null;
            });
            if (!validStart)
                return;
            ITrackingPoint closingPoint = null;
            _guardedOperations.CallExtensionPoint(() => closingPoint = session.ClosingPoint);
            HighlightSpan(closingPoint);
            _stack.Push(session);
        }

        public ReadOnlyObservableCollection<IBraceCompletionSession> Sessions => new ReadOnlyObservableCollection<IBraceCompletionSession>(new ObservableCollection<IBraceCompletionSession>(_stack));

        public void RemoveOutOfRangeSessions(SnapshotPoint point)
        {
            bool flag = false;
            while (_stack.Count > 0 && !Contains(TopSession, point))
            {
                flag = true;
                PopSession();
            }
            if (!flag)
                return;
            HighlightSpan(TopSession?.ClosingPoint);
        }

        public void Clear()
        {
            while (_stack.Count > 0)
                PopSession();
            SetCurrentBuffer(null);
            HighlightSpan(null);
        }

        private void RegisterEvents()
        {
            if (_adornmentServiceFactory != null)
                _adornmentService = _adornmentServiceFactory.GetOrCreateService(_textView);
            _textView.Caret.PositionChanged += Caret_PositionChanged;
            _textView.Closed += TextView_Closed;
        }

        private void UnregisterEvents()
        {
            _textView.Caret.PositionChanged -= Caret_PositionChanged;
            _textView.Closed -= TextView_Closed;
            SetCurrentBuffer(null);
            _textView = null;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
            subjectBuffer.PostChanged += SubjectBuffer_PostChanged;
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
            subjectBuffer.PostChanged -= SubjectBuffer_PostChanged;
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            UnregisterEvents();
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            if (_stack.Count <= 0)
                return;
            if (_currentSubjectBuffer != null && e.TextView.TextBuffer != _currentSubjectBuffer)
            {
                SnapshotPoint? point = e.NewPosition.Point.GetPoint(_currentSubjectBuffer, PositionAffinity.Successor);
                if (point.HasValue)
                    RemoveOutOfRangeSessions(point.Value);
                else
                    _stack.Clear();
            }
            else
                RemoveOutOfRangeSessions(e.NewPosition.BufferPosition);
        }

        private void SubjectBuffer_PostChanged(object sender, EventArgs e)
        {
            bool flag = false;
            while (_stack.Count > 0 && !IsSessionValid(TopSession))
            {
                flag = true;
                _stack.Pop().Finish();
            }
            if (!flag)
                return;
            ITrackingPoint closingPoint = null;
            if (TopSession != null)
                _guardedOperations.CallExtensionPoint(() => closingPoint = TopSession.ClosingPoint);
            HighlightSpan(closingPoint);
        }

        private bool IsSessionValid(IBraceCompletionSession session)
        {
            bool isValid = false;
            _guardedOperations.CallExtensionPoint(() =>
            {
                if (session.ClosingPoint == null || session.OpeningPoint == null || session.SubjectBuffer == null)
                    return;
                ITextSnapshot currentSnapshot = session.SubjectBuffer.CurrentSnapshot;
                SnapshotPoint point1 = session.ClosingPoint.GetPoint(currentSnapshot);
                SnapshotPoint point2 = session.OpeningPoint.GetPoint(currentSnapshot);
                isValid = point1.Position > 1 && point2.Position <= point1.Position - 2 && point2.GetChar() == session.OpeningBrace && point1.Subtract(1).GetChar() == session.ClosingBrace;
            });
            return isValid;
        }

        private void SetCurrentBuffer(ITextBuffer buffer)
        {
            if (_currentSubjectBuffer == buffer)
                return;
            if (_currentSubjectBuffer != null)
                DisconnectSubjectBuffer(_currentSubjectBuffer);
            _currentSubjectBuffer = buffer;
            if (_currentSubjectBuffer == null)
                return;
            ConnectSubjectBuffer(_currentSubjectBuffer);
        }

        private void PopSession()
        {
            IBraceCompletionSession session = _stack.Pop();
            ITextBuffer nextSubjectBuffer = null;
            _guardedOperations.CallExtensionPoint(() =>
            {
                session.Finish();
                if (TopSession == null)
                    return;
                nextSubjectBuffer = TopSession.SubjectBuffer;
            });
            SetCurrentBuffer(nextSubjectBuffer);
        }

        private void HighlightSpan(ITrackingPoint point)
        {
            if (_adornmentService == null)
                return;
            _adornmentService.Point = point;
        }

        private bool Contains(IBraceCompletionSession session, SnapshotPoint point)
        {
            bool contains = false;
            _guardedOperations.CallExtensionPoint(() =>
            {
                if (session.OpeningPoint == null || session.ClosingPoint == null || (session.OpeningPoint.TextBuffer != session.ClosingPoint.TextBuffer || point.Snapshot.TextBuffer != session.OpeningPoint.TextBuffer))
                    return;
                ITextSnapshot snapshot = point.Snapshot;
                contains = session.OpeningPoint.GetPosition(snapshot) < point.Position && session.ClosingPoint.GetPosition(snapshot) > point.Position;
            });
            return contains;
        }
    }
}