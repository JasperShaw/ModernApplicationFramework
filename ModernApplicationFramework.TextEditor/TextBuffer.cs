using System;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Text.Differencing;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class TextBuffer : BaseBuffer
    {
        private readonly bool _spurnGroup;

        public TextBuffer(IContentType contentType, StringRebuilder content, ITextDifferencingService textDifferencingService, GuardedOperations guardedOperations)
            : this(contentType, content, textDifferencingService, guardedOperations, false)
        {
        }

        public TextBuffer(IContentType contentType, StringRebuilder content, ITextDifferencingService textDifferencingService, GuardedOperations guardedOperations, bool spurnGroup)
            : base(contentType, content.Length, textDifferencingService, guardedOperations)
        {
            Group = new BufferGroup(this);
            Builder = content;
            _spurnGroup = spurnGroup;
            CurrentSnapshotProtected = TakeSnapshot();
        }

        public ITextSnapshot ReloadContent(StringRebuilder newContent, EditOptions editOptions, object editTag)
        {
            using (var reloadEdit = new ReloadEdit(this, CurrentSnapshotProtected, editOptions, editTag))
                return reloadEdit.ReloadContent(newContent);
        }


        internal TextContentChangedEventArgs ApplyReload(StringRebuilder newContent, EditOptions editOptions,
            object editTag)
        {
            ITextSnapshot currentSnapshot = CurrentSnapshotProtected;
            var textChange = TextChange.Create(0, BufferFactoryService.StringRebuilderFromSnapshotSpan(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length)), newContent, currentSnapshot);
            var next = CurrentVersion.CreateNext(null, newContent.Length);
            var textSnapshot = new TextSnapshot(this, next, newContent);
            CurrentVersion.SetChanges(NormalizedTextChangeCollection.Create(new[]
            {
                textChange
            }, editOptions.ComputeMinimalChange ? editOptions.DifferenceOptions : new StringDifferenceOptions?(), TextDifferencingService, currentSnapshot, textSnapshot));
            CurrentVersion = next;
            Builder = newContent;
            CurrentSnapshotProtected = textSnapshot;
            return new TextContentChangedEventArgs(currentSnapshot, textSnapshot, editOptions, editTag);
        }

        public override ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            return new BasicEdit(this, CurrentSnapshotProtected, options, reiteratedVersionNumber, editTag);
        }

        protected internal override ISubordinateTextEdit CreateSubordinateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            return new BasicEdit(this, CurrentSnapshotProtected, options, reiteratedVersionNumber, editTag);
        }

        private ITextEventRaiser ApplyChangesAndSetSnapshot(FrugalList<TextChange> changes, EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            var normalizedChanges = NormalizedTextChangeCollection.Create(changes, options.ComputeMinimalChange ? options.DifferenceOptions : new StringDifferenceOptions?(), TextDifferencingService);
            var currentSnapshot1 = CurrentSnapshot;
            SetCurrentVersionAndSnapshot(normalizedChanges, reiteratedVersionNumber ?? -1);
            var currentSnapshot2 = CurrentSnapshot;
            var options1 = options;
            var editTag1 = editTag;
            return new TextContentChangedEventRaiser(currentSnapshot1, currentSnapshot2, options1, editTag1);
        }

        protected override BaseSnapshot TakeSnapshot()
        {
            return new TextSnapshot(this, CurrentVersion, Builder);
        }

        private class BasicEdit : Edit, ISubordinateTextEdit
        {
            private readonly TextBuffer _textBuffer;
            private bool _subordinate;

            public BasicEdit(TextBuffer textBuffer, ITextSnapshot originSnapshot, EditOptions options, int? reiteratedVersionNumber, object editTag)
              : base(textBuffer, originSnapshot, options, reiteratedVersionNumber, editTag)
            {
                _textBuffer = textBuffer;
                _subordinate = true;
            }

            public ITextBuffer TextBuffer => _textBuffer;

            protected override ITextSnapshot PerformApply()
            {
                CheckActive();
                Applied = true;
                _subordinate = false;
                var currentSnapshot = _textBuffer.CurrentSnapshot;
                if (Changes.Count > 0)
                {
                    if (_textBuffer.Group.Members.Count == 1 || _textBuffer._spurnGroup)
                    {
                        void Action()
                        {
                        }

                        if (!CheckForCancellation(Action)) return currentSnapshot;
                        FinalApply();
                        currentSnapshot = _textBuffer.CurrentSnapshot;
                    }
                    else
                    {
                        _textBuffer.Group.PerformMasterEdit(_textBuffer, this, Options, EditTag);
                        if (!Canceled)
                            currentSnapshot = _textBuffer.CurrentSnapshot;
                    }
                }
                else
                    BaseBuffer.EditInProgressProtected = false;
                return currentSnapshot;
            }

            public void PreApply()
            {
            }

            public void FinalApply()
            {
                if (Changes.Count > 0)
                {
                    var raiser = _textBuffer.ApplyChangesAndSetSnapshot(Changes, Options, ReiteratedVersionNumber, EditTag);
                    BaseBuffer.Group.EnqueueEvents(raiser, BaseBuffer);
                    raiser.RaiseEvent(BaseBuffer, true);
                }
                BaseBuffer.EditInProgressProtected = false;
                if (!_subordinate)
                    return;
                BaseBuffer.Group.FinishEdit();
            }
        }

        private class ReloadEdit : TextBufferBaseEdit, ISubordinateTextEdit
        {
            private StringRebuilder _newContent;
            private readonly TextBuffer _textBuffer;
            private readonly ITextSnapshot _originSnapshot;
            private readonly object _editTag;
            private readonly EditOptions _editOptions;
            private TextContentChangingEventArgs _raisedChangingEventArgs;

            public ReloadEdit(TextBuffer textBuffer, ITextSnapshot originSnapshot, EditOptions editOptions, object editTag)
              : base(textBuffer)
            {
                _textBuffer = textBuffer;
                _originSnapshot = originSnapshot;
                _editOptions = editOptions;
                _editTag = editTag;
            }

            public ITextSnapshot ReloadContent(StringRebuilder newContent)
            {
                if (BaseBuffer.IsReadOnlyImplementation(new Span(0, _originSnapshot.Length), true))
                {
                    Applied = true;
                    BaseBuffer.EditInProgressProtected = false;
                    BaseBuffer.Group.FinishEdit();
                    return _originSnapshot;
                }
                _newContent = newContent;
                BaseBuffer.Group.PerformMasterEdit(_textBuffer, this, _editOptions, _editTag);
                BaseBuffer.Group.FinishEdit();
                return _textBuffer.CurrentSnapshot;
            }

            public void PreApply()
            {
            }

            public bool CheckForCancellation(Action cancelationResponse)
            {
                if (_raisedChangingEventArgs == null)
                {
                    _raisedChangingEventArgs = new TextContentChangingEventArgs(_originSnapshot, _editTag, args => Cancel());
                    BaseBuffer.RaiseChangingEvent(_raisedChangingEventArgs);
                }
                Canceled = _raisedChangingEventArgs.Canceled;
                return !_raisedChangingEventArgs.Canceled;
            }

            public void FinalApply()
            {
                _textBuffer.ApplyReload(_newContent, _editOptions, _editTag);
                var changedEventRaiser = new TextContentChangedEventRaiser(_originSnapshot, BaseBuffer.CurrentSnapshotProtected, _editOptions, _editTag);
                Applied = true;
                BaseBuffer.Group.EnqueueEvents(changedEventRaiser, BaseBuffer);
                changedEventRaiser.RaiseEvent(BaseBuffer, true);
                BaseBuffer.EditInProgressProtected = false;
            }

            public ITextBuffer TextBuffer => BaseBuffer;

            public void RecordMasterChangeOffset(int masterChangeOffset)
            {
                throw new InvalidOperationException("Reloads should not be getting offsets from any other change.");
            }
        }
    }
}
