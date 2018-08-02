using System;
using System.Collections.Generic;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class ElisionBuffer : BaseProjectionBuffer, IElisionBuffer
    {
        private ElisionMap _content;
        private ElisionSnapshot _currentElisionSnapshot;
        private ITextSnapshot _sourceSnapshot;


        public ElisionBuffer(IProjectionEditResolver resolver, IContentType contentType, ITextBuffer sourceBuffer, NormalizedSpanCollection exposedSpans, ElisionBufferOptions options, ITextDifferencingService textDifferencingService, GuardedOperations guardedOperations)
            : base(resolver, contentType, textDifferencingService, guardedOperations)
        {
            SourceBuffer = sourceBuffer;
            _sourceSnapshot = sourceBuffer.CurrentSnapshot;
            var sourceBuffer1 = (BaseBuffer)sourceBuffer;
            Group = sourceBuffer1.Group;
            Group.AddMember(this);
            _content = new ElisionMap(_sourceSnapshot, exposedSpans);
            var stringRebuilder = StringRebuilder.Empty;
            foreach (var span in exposedSpans)
                stringRebuilder = stringRebuilder.Append(BufferFactoryService.StringRebuilderFromSnapshotAndSpan(_sourceSnapshot, span));

            Builder = stringRebuilder;
            Options = options;
            CurrentVersion.SetLength(_content.Length);
            _currentElisionSnapshot = new ElisionSnapshot(this, _sourceSnapshot, CurrentVersion, Builder, _content, (uint)(options & ElisionBufferOptions.FillInMappingMode) > 0U);
            CurrentSnapshotProtected = _currentElisionSnapshot;
        }

        public override IList<ITextBuffer> SourceBuffers => new FrugalList<ITextBuffer>
        {
            SourceBuffer
        };

        public ITextBuffer SourceBuffer { get; }

        public ElisionBufferOptions Options { get; }

        public IProjectionSnapshot ElideSpans(NormalizedSpanCollection spansToElide)
        {
            if (spansToElide == null)
                throw new ArgumentNullException(nameof(spansToElide));
            return ModifySpans(spansToElide, null);
        }

        public IProjectionSnapshot ExpandSpans(NormalizedSpanCollection spansToExpand)
        {
            if (spansToExpand == null)
                throw new ArgumentNullException(nameof(spansToExpand));
            return ModifySpans(null, spansToExpand);
        }

        public IProjectionSnapshot ModifySpans(NormalizedSpanCollection spansToElide, NormalizedSpanCollection spansToExpand)
        {
            using (var spanEdit = new SpanEdit(this))
                return spanEdit.Apply(spansToElide, spansToExpand);
        }

        public override ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            return new ElisionEdit(this, _currentElisionSnapshot, options, reiteratedVersionNumber, editTag);
        }

        protected internal override ISubordinateTextEdit CreateSubordinateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            return new ElisionEdit(this, _currentElisionSnapshot, options, reiteratedVersionNumber, editTag);
        }

        internal void ComputeSourceEdits(FrugalList<TextChange> changes)
        {
            ITextEdit edit = Group.GetEdit((BaseBuffer)SourceBuffer);
            foreach (var change in changes)
            {
                if (change.OldLength > 0)
                {
                    foreach (var sourceSnapshot in (IEnumerable<SnapshotSpan>)_currentElisionSnapshot.MapToSourceSnapshots(new Span(change.OldPosition, change.OldLength)))
                        edit.Delete(sourceSnapshot);
                }
                if (change.NewLength > 0)
                {
                    var sourceSnapshots = _currentElisionSnapshot.MapInsertionPointToSourceSnapshots(change.OldPosition, null);
                    if (sourceSnapshots.Count == 1)
                    {
                        edit.Insert(sourceSnapshots[0].Position, change.NewText);
                    }
                    else
                    {
                        var numArray = new int[sourceSnapshots.Count];
                        Resolver?.FillInInsertionSizes(new SnapshotPoint(_currentElisionSnapshot, change.OldPosition), sourceSnapshots, change.NewText, numArray);
                        var start = 0;
                        for (var index = 0; index < numArray.Length; ++index)
                        {
                            var length = index == numArray.Length - 1 ? change.NewLength - start : Math.Min(numArray[index], change.NewLength - start);
                            if (length > 0)
                            {
                                edit.Insert(sourceSnapshots[index].Position, TextChange.ChangeNewSubstring(change, start, length));
                                start += length;
                                if (start == change.NewLength)
                                    break;
                            }
                        }
                    }
                }
            }
            EditApplicationInProgress = true;
        }

        public override ITextEventRaiser PropagateSourceChanges(EditOptions options, object editTag)
        {
            var changedEventRaiser = IncorporateChanges();
            changedEventRaiser.RaiseEvent(this, true);
            return changedEventRaiser;
        }

        private ElisionSourceSpansChangedEventArgs ApplySpanChanges(NormalizedSpanCollection spansToElide, NormalizedSpanCollection spansToExpand)
        {
            var currentElisionSnapshot = _currentElisionSnapshot;
            var elisionMap = _content.EditSpans(_sourceSnapshot, spansToElide, spansToExpand, out var textChanges);
            if (elisionMap == _content)
                return null;
            _content = elisionMap;
            SetCurrentVersionAndSnapshot(NormalizedTextChangeCollection.Create(textChanges));
            return new ElisionSourceSpansChangedEventArgs(currentElisionSnapshot, _currentElisionSnapshot, spansToElide, spansToExpand, null);
        }

        public override IProjectionSnapshot CurrentSnapshot => _currentElisionSnapshot;

        protected override BaseProjectionSnapshot CurrentBaseSnapshot => _currentElisionSnapshot;

        IElisionSnapshot IElisionBuffer.CurrentSnapshot => _currentElisionSnapshot;

        protected override StringRebuilder GetDoppelgangerBuilder()
        {
            var sourceSnapshot = _sourceSnapshot;
            if (CurrentVersion.Length == sourceSnapshot.Length && sourceSnapshot is BaseSnapshot)
                return BufferFactoryService.StringRebuilderFromSnapshotAndSpan(sourceSnapshot, new Span(0, sourceSnapshot.Length));
            return null;
        }

        protected override BaseSnapshot TakeSnapshot()
        {
            _currentElisionSnapshot = new ElisionSnapshot(this, _sourceSnapshot, CurrentVersion, Builder, _content, (uint)(Options & ElisionBufferOptions.FillInMappingMode) > 0U);
            return _currentElisionSnapshot;
        }

        private TextContentChangedEventRaiser IncorporateChanges()
        {
            var projectedChanges = new FrugalList<TextChange>();
            TextContentChangedEventArgs contentChangedEventArg = PendingContentChangedEventArgs[0];
            INormalizedTextChangeCollection changes;
            if (PendingContentChangedEventArgs.Count == 1)
            {
                changes = contentChangedEventArg.Changes;
                _sourceSnapshot = contentChangedEventArg.After;
            }
            else
            {
                var denormChangesWithSentinel = new List<TextChange>
                {
                    new TextChange(int.MaxValue, StringRebuilder.Empty, StringRebuilder.Empty, LineBreakBoundaryConditions.None)
                };
                foreach (var changed in PendingContentChangedEventArgs)
                    NormalizedTextChangeCollection.Denormalize(changed.Changes, denormChangesWithSentinel);

                var frugalList = new FrugalList<TextChange>();
                for (var index = 0; index < denormChangesWithSentinel.Count - 1; ++index)
                    frugalList.Add(denormChangesWithSentinel[index]);
                changes = NormalizedTextChangeCollection.Create(frugalList);
                _sourceSnapshot = PendingContentChangedEventArgs[PendingContentChangedEventArgs.Count - 1].After;
            }
            if (changes.Count > 0)
                _content = _content.IncorporateChanges(changes, projectedChanges, contentChangedEventArg.Before, _sourceSnapshot, _currentElisionSnapshot);
            PendingContentChangedEventArgs.Clear();
            var currentElisionSnapshot1 = _currentElisionSnapshot;
            SetCurrentVersionAndSnapshot(NormalizedTextChangeCollection.Create(projectedChanges), -1);
            EditApplicationInProgress = false;
            var currentElisionSnapshot2 = _currentElisionSnapshot;
            var options = contentChangedEventArg.Options;
            var editTag = contentChangedEventArg.EditTag;
            return new TextContentChangedEventRaiser(currentElisionSnapshot1, currentElisionSnapshot2, options, editTag);
        }

        internal override void OnSourceTextChanged(object sender, TextContentChangedEventArgs e)
        {
            PendingContentChangedEventArgs.Add(e);
            if (EditApplicationInProgress)
                return;
            Group.ScheduleIndependentEdit(this);
        }

        public event EventHandler<ElisionSourceSpansChangedEventArgs> SourceSpansChanged;

        private class ElisionEdit : Edit, ISubordinateTextEdit
        {
            private readonly ElisionBuffer _elisionBuffer;
            private bool _subordinate;

            public ElisionEdit(ElisionBuffer elisionBuffer, ITextSnapshot originSnapshot, EditOptions options, int? reiteratedVersionNumber, object editTag)
                : base(elisionBuffer, originSnapshot, options, reiteratedVersionNumber, editTag)
            {
                _elisionBuffer = elisionBuffer;
                _subordinate = true;
            }

            public ITextBuffer TextBuffer => _elisionBuffer;

            protected override ITextSnapshot PerformApply()
            {
                CheckActive();
                Applied = true;
                _subordinate = false;
                var currentSnapshot = (ITextSnapshot)BaseBuffer.CurrentSnapshotProtected;
                if (Changes.Count > 0)
                {
                    _elisionBuffer.Group.PerformMasterEdit(_elisionBuffer, this, Options, EditTag);
                    if (!Canceled)
                        currentSnapshot = BaseBuffer.CurrentSnapshotProtected;
                }
                else
                    BaseBuffer.EditInProgressProtected = false;
                return currentSnapshot;
            }

            public void PreApply()
            {
                if (Changes.Count <= 0)
                    return;
                _elisionBuffer.ComputeSourceEdits(Changes);
            }

            public void FinalApply()
            {
                if (Changes.Count > 0 || _elisionBuffer.PendingContentChangedEventArgs.Count > 0)
                {
                    _elisionBuffer.Group.CancelIndependentEdit(_elisionBuffer);
                    var changedEventRaiser = _elisionBuffer.IncorporateChanges();
                    BaseBuffer.Group.EnqueueEvents(changedEventRaiser, BaseBuffer);
                    changedEventRaiser.RaiseEvent(BaseBuffer, true);
                }
                _elisionBuffer.EditInProgressProtected = false;
                _elisionBuffer.EditApplicationInProgress = false;
                if (!_subordinate)
                    return;
                _elisionBuffer.Group.FinishEdit();
            }

            public override void CancelApplication()
            {
                if (Canceled)
                    return;
                base.CancelApplication();
                _elisionBuffer.EditApplicationInProgress = false;
                _elisionBuffer.PendingContentChangedEventArgs.Clear();
            }
        }

        private class ElisionSourceSpansChangedEventRaiser : ITextEventRaiser
        {
            private readonly ElisionSourceSpansChangedEventArgs _args;

            public ElisionSourceSpansChangedEventRaiser(ElisionSourceSpansChangedEventArgs args)
            {
                _args = args;
            }

            public void RaiseEvent(BaseBuffer baseBuffer, bool immediate)
            {
                var sourceSpansChanged = ((ElisionBuffer)baseBuffer).SourceSpansChanged;
                sourceSpansChanged?.Invoke(this, _args);
                baseBuffer.RawRaiseEvent(_args, immediate);
            }

            public bool HasPostEvent => false;
        }

        private class SpanEdit : TextBufferBaseEdit
        {
            private readonly ElisionBuffer _elBuffer;

            public SpanEdit(ElisionBuffer elBuffer)
                : base(elBuffer)
            {
                _elBuffer = elBuffer;
            }

            public IProjectionSnapshot Apply(NormalizedSpanCollection spansToElide, NormalizedSpanCollection spansToExpand)
            {
                Applied = true;
                try
                {
                    if (spansToElide == null)
                        spansToElide = NormalizedSpanCollection.Empty;
                    if (spansToExpand == null)
                        spansToExpand = NormalizedSpanCollection.Empty;
                    if (spansToElide.Count > 0 || spansToExpand.Count > 0)
                    {
                        if (spansToElide.Count > 0 && spansToElide[spansToElide.Count - 1].End > _elBuffer._sourceSnapshot.Length)
                            throw new ArgumentOutOfRangeException(nameof(spansToElide));
                        if (spansToExpand.Count > 0 && spansToExpand[spansToExpand.Count - 1].End > _elBuffer._sourceSnapshot.Length)
                            throw new ArgumentOutOfRangeException(nameof(spansToExpand));
                        var args = _elBuffer.ApplySpanChanges(spansToElide, spansToExpand);
                        if (args != null)
                        {
                            var changedEventRaiser = new ElisionSourceSpansChangedEventRaiser(args);
                            BaseBuffer.Group.EnqueueEvents(changedEventRaiser, BaseBuffer);
                            changedEventRaiser.RaiseEvent(BaseBuffer, true);
                        }
                        BaseBuffer.EditInProgressProtected = false;
                    }
                    else
                        BaseBuffer.EditInProgressProtected = false;
                }
                finally
                {
                    BaseBuffer.Group.FinishEdit();
                }
                return _elBuffer._currentElisionSnapshot;
            }
        }
    }
}