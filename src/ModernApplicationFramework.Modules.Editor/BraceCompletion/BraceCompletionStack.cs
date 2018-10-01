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
        private readonly IBraceCompletionAdornmentServiceFactory _adornmentServiceFactory;
        private readonly Stack<IBraceCompletionSession> _stack;
        private ITextView _textView;
        private readonly IGuardedOperations _guardedOperations;
        private IBraceCompletionAdornmentService _adornmentService;
        private ITextBuffer _currentSubjectBuffer;

        public IBraceCompletionSession TopSession => _stack.Count <= 0 ? null : _stack.Peek();

        public ReadOnlyObservableCollection<IBraceCompletionSession> Sessions =>
            new ReadOnlyObservableCollection<IBraceCompletionSession>(
                new ObservableCollection<IBraceCompletionSession>(_stack));

        public BraceCompletionStack(ITextView textView, IBraceCompletionAdornmentServiceFactory adornmentFactory, IGuardedOperations guardedOperations)
        {
            _adornmentServiceFactory = adornmentFactory;
            _stack = new Stack<IBraceCompletionSession>();
            _textView = textView;
            _guardedOperations = guardedOperations;
            RegisterEvents();
        }


        public void PushSession(IBraceCompletionSession session)
        {
            var view = (ITextView)null;
            var buffer = (ITextBuffer)null;
            _guardedOperations.CallExtensionPoint(() =>
            {
                view = session.TextView;
                buffer = session.SubjectBuffer;
            });
            if (view == null || buffer == null)
                return;
            SetCurrentBuffer(buffer);
            var validStart = false;
            _guardedOperations.CallExtensionPoint(() =>
            {
                session.Start();
                validStart = session.OpeningPoint != null && session.ClosingPoint != null;
            });
            if (!validStart)
                return;
            var closingPoint = (ITrackingPoint)null;
            _guardedOperations.CallExtensionPoint(() => closingPoint = session.ClosingPoint);
            HighlightSpan(closingPoint);
            _stack.Push(session);
        }

        public void RemoveOutOfRangeSessions(SnapshotPoint point)
        {
            var flag = false;
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

        private void PopSession()
        {
            var session = _stack.Pop();
            var nextSubjectBuffer = (ITextBuffer)null;
            _guardedOperations.CallExtensionPoint(() =>
            {
                session.Finish();
                if (TopSession == null)
                    return;
                nextSubjectBuffer = TopSession.SubjectBuffer;
            });
            SetCurrentBuffer(nextSubjectBuffer);
        }

        private void UnregisterEvents()
        {
            _textView.Caret.PositionChanged -= Caret_PositionChanged;
            _textView.Closed -= TextView_Closed;
            SetCurrentBuffer(null);
            _textView = null;
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

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
            subjectBuffer.PostChanged += SubjectBuffer_PostChanged;
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
            subjectBuffer.PostChanged -= SubjectBuffer_PostChanged;
        }

        private void SubjectBuffer_PostChanged(object sender, EventArgs e)
        {
            var flag = false;
            while (_stack.Count > 0 && !IsSessionValid(TopSession))
            {
                flag = true;
                _stack.Pop().Finish();
            }
            if (!flag)
                return;
            var closingPoint = (ITrackingPoint)null;
            if (TopSession != null)
                _guardedOperations.CallExtensionPoint(() => closingPoint = TopSession.ClosingPoint);
            HighlightSpan(closingPoint);
        }

        private bool IsSessionValid(IBraceCompletionSession session)
        {
            var isValid = false;
            _guardedOperations.CallExtensionPoint(() =>
            {
                if (session.ClosingPoint == null || session.OpeningPoint == null || session.SubjectBuffer == null)
                    return;
                var currentSnapshot = session.SubjectBuffer.CurrentSnapshot;
                var point1 = session.ClosingPoint.GetPoint(currentSnapshot);
                var point2 = session.OpeningPoint.GetPoint(currentSnapshot);
                isValid = point1.Position > 1 && point2.Position <= point1.Position - 2 && point2.GetChar() == session.OpeningBrace && point1.Subtract(1).GetChar() == session.ClosingBrace;
            });
            return isValid;
        }

        private void HighlightSpan(ITrackingPoint point)
        {
            if (_adornmentService == null)
                return;
            _adornmentService.Point = point;
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            UnregisterEvents();
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            if(_stack.Count <= 0)
            return;
            if (_currentSubjectBuffer != null && e.TextView.TextBuffer != _currentSubjectBuffer)
            {
                var point = e.NewPosition.Point.GetPoint(_currentSubjectBuffer, PositionAffinity.Successor);
                if (point.HasValue)
                    RemoveOutOfRangeSessions(point.Value);
                else
                    _stack.Clear();
            }
            else
                RemoveOutOfRangeSessions(e.NewPosition.BufferPosition);
        }

        private bool Contains(IBraceCompletionSession session, SnapshotPoint point)
        {
            var contains = false;
            _guardedOperations.CallExtensionPoint(() =>
            {
                if (session.OpeningPoint == null || session.ClosingPoint == null || (session.OpeningPoint.TextBuffer != session.ClosingPoint.TextBuffer || point.Snapshot.TextBuffer != session.OpeningPoint.TextBuffer))
                    return;
                var snapshot = point.Snapshot;
                contains = session.OpeningPoint.GetPosition(snapshot) < point.Position && session.ClosingPoint.GetPosition(snapshot) > point.Position;
            });
            return contains;
        }
    }
}