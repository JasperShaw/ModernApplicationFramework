using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ModernApplicationFramework.Modules.Editor.Implementation;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal abstract class BaseBuffer : ITextBuffer
    {
        protected internal StringRebuilder Builder;
        protected internal BaseSnapshot CurrentSnapshotProtected;
        protected internal bool EditInProgressProtected;
        protected internal BufferGroup Group;
        protected ReadOnlySpanCollection ReadOnlyRegionSpanCollection;
        protected ITextDifferencingService TextDifferencingService;
        private readonly GuardedOperations _guardedOperations;
        private readonly object _syncLock = new object();
        private Thread _editThread;

        private PropertyCollection _properties;
        private FrugalList<IReadOnlyRegion> _readOnlyRegions;

        public event EventHandler<TextContentChangedEventArgs> Changed;
        public event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;

        public event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;

        //public event EventHandler<TextContentChangedEventArgs> ChangedOnBackground;
        public event EventHandler<TextContentChangingEventArgs> Changing;
        public event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;
        public event EventHandler PostChanged;
        public event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;

        internal event EventHandler<TextContentChangedEventArgs> ChangedImmediate;
        internal event EventHandler<ContentTypeChangedEventArgs> ContentTypeChangedImmediate;

        public IContentType ContentType { get; private set; }

        public ITextSnapshot CurrentSnapshot => CurrentSnapshotProtected;

        public bool EditInProgress => EditInProgressProtected;

        public PropertyCollection Properties
        {
            get
            {
                if (_properties == null)
                    lock (_syncLock)
                    {
                        if (_properties == null)
                            _properties = new PropertyCollection();
                    }

                return _properties;
            }
        }

        protected TextVersion CurrentVersion { get; set; }

        protected BaseBuffer(IContentType contentType, int initialLength,
            ITextDifferencingService textDifferencingService,
            GuardedOperations guardedOperations)
        {
            ContentType = contentType;
            CurrentVersion = new TextVersion(this, new TextImageVersion(initialLength));
            TextDifferencingService = textDifferencingService;
            _guardedOperations = guardedOperations;
        }

        internal interface ITextEventRaiser
        {
            bool HasPostEvent { get; }
            void RaiseEvent(BaseBuffer baseBuffer, bool immediate);
        }

        public StringRebuilder ApplyChangesToStringRebuilder(INormalizedTextChangeCollection normalizedChanges,
            StringRebuilder source)
        {
            var doppelgangerBuilder = GetDoppelgangerBuilder();
            if (doppelgangerBuilder != null)
                return doppelgangerBuilder;
            for (var index = normalizedChanges.Count - 1; index >= 0; --index)
            {
                var normalizedChange = normalizedChanges[index];
                source = source.Replace(normalizedChange.OldSpan, TextChange.NewStringRebuilder(normalizedChange));
            }

            return source;
        }

        public void ChangeContentType(IContentType newContentType, object editTag)
        {
            if (newContentType == null)
                throw new ArgumentNullException(nameof(newContentType));
            if (newContentType == ContentType)
                return;
            using (var contentTypeEdit = new ContentTypeEdit(this, CurrentSnapshotProtected, editTag, newContentType))
            {
                contentTypeEdit.Apply();
            }
        }

        public bool CheckEditAccess()
        {
            if (_editThread != null)
                return _editThread == Thread.CurrentThread;
            return true;
        }

        public abstract ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag);

        public ITextEdit CreateEdit()
        {
            return CreateEdit(EditOptions.None, new int?(), null);
        }

        public IReadOnlyRegionEdit CreateReadOnlyRegionEdit()
        {
            return CreateReadOnlyRegionEdit(null);
        }

        public IReadOnlyRegionEdit CreateReadOnlyRegionEdit(object editTag)
        {
            return new ReadOnlyRegionEdit(this, CurrentSnapshot, editTag);
        }

        public ITextSnapshot Delete(Span deleteSpan)
        {
            using (var edit = CreateEdit())
            {
                edit.Delete(deleteSpan);
                return edit.Apply();
            }
        }

        public NormalizedSpanCollection GetReadOnlyExtents(Span span)
        {
            ReadOnlyQueryThreadCheck();
            if (span.End > CurrentSnapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            return GetReadOnlyExtentsImplementation(span);
        }

        public ITextSnapshot Insert(int position, string text)
        {
            using (var edit = CreateEdit())
            {
                edit.Insert(position, text);
                return edit.Apply();
            }
        }

        public bool IsReadOnly(int position)
        {
            return IsReadOnly(position, false);
        }

        public bool IsReadOnly(int position, bool isEdit)
        {
            ReadOnlyQueryThreadCheck();
            if (position < 0 || position > CurrentSnapshotProtected.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            return IsReadOnlyImplementation(position, isEdit);
        }

        public bool IsReadOnly(Span span)
        {
            return IsReadOnly(span, false);
        }

        public bool IsReadOnly(Span span, bool isEdit)
        {
            ReadOnlyQueryThreadCheck();
            if (span.End > CurrentSnapshotProtected.Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            return IsReadOnlyImplementation(span, isEdit);
        }

        public ITextSnapshot Replace(Span replaceSpan, string replaceWith)
        {
            using (var edit = CreateEdit())
            {
                edit.Replace(replaceSpan, replaceWith);
                return edit.Apply();
            }
        }

        public void TakeThreadOwnership()
        {
            lock (_syncLock)
            {
                if (_editThread != null && _editThread != Thread.CurrentThread)
                    throw new InvalidOperationException();
                _editThread = Thread.CurrentThread;
            }
        }

        internal void RaiseChangingEvent(TextContentChangingEventArgs args)
        {
            var changing = Changing;
            if (changing == null)
                return;
            foreach (EventHandler<TextContentChangingEventArgs> invocation in changing.GetInvocationList())
            {
                try
                {
                    invocation(this, args);
                }
                catch (Exception ex)
                {
                    _guardedOperations.HandleException(invocation, ex);
                }

                if (args.Canceled)
                    break;
            }
        }

        internal void RaisePostChangedEvent()
        {
            _guardedOperations.RaiseEvent(this, PostChanged);
        }

        internal void RawRaiseEvent(TextContentChangedEventArgs args, bool immediate)
        {
            if (immediate)
            {
                var changedImminate = ChangedImmediate;
                if (changedImminate == null)
                    return;
                changedImminate(this, args);
            }
            else
            {
                var changedHighPriority = ChangedHighPriority;
                var changed = Changed;
                var changedLowPriority = ChangedLowPriority;
                if (changedHighPriority != null)
                    _guardedOperations.RaiseEvent(this, changedHighPriority, args);
                if (changed != null)
                    _guardedOperations.RaiseEvent(this, changed, args);
                if (changedLowPriority != null)
                    _guardedOperations.RaiseEvent(this, changedLowPriority, args);
            }
        }

        protected internal abstract ISubordinateTextEdit CreateSubordinateEdit(EditOptions options,
            int? reiteratedVersionNumber, object editTag);

        protected internal virtual NormalizedSpanCollection GetReadOnlyExtentsImplementation(Span span)
        {
            var frugalList = new FrugalList<Span>();
            if (ReadOnlyRegionSpanCollection == null)
                return new NormalizedSpanCollection(frugalList);
            foreach (var effectiveReadOnlySpan in ReadOnlyRegionSpanCollection.QueryAllEffectiveReadOnlySpans(
                CurrentVersion))
            {
                Span span1 = effectiveReadOnlySpan.GetSpan(CurrentSnapshotProtected);
                var nullable = span1 == span ? span1 : span1.Overlap(span);
                if (nullable.HasValue)
                    frugalList.Add(nullable.Value);
            }

            return new NormalizedSpanCollection(frugalList);
        }

        protected internal virtual bool IsReadOnlyImplementation(int position, bool isEdit)
        {
            if (ReadOnlyRegionSpanCollection == null)
                return false;
            return ReadOnlyRegionSpanCollection.IsReadOnly(position, CurrentSnapshotProtected, isEdit);
        }

        protected internal virtual bool IsReadOnlyImplementation(Span span, bool isEdit)
        {
            if (ReadOnlyRegionSpanCollection == null)
                return false;
            return ReadOnlyRegionSpanCollection.IsReadOnly(span, CurrentSnapshotProtected, isEdit);
        }

        protected virtual StringRebuilder GetDoppelgangerBuilder()
        {
            return null;
        }

        protected void SetCurrentVersionAndSnapshot(INormalizedTextChangeCollection normalizedChanges,
            int reiteratedVersionNumber = -1)
        {
            CurrentVersion = CurrentVersion.CreateNext(normalizedChanges, -1, reiteratedVersionNumber);
            Builder = ApplyChangesToStringRebuilder(normalizedChanges, Builder);
            CurrentSnapshotProtected = TakeSnapshot();
        }

        protected abstract BaseSnapshot TakeSnapshot();

        private void ReadOnlyQueryThreadCheck()
        {
            if (!CheckEditAccess())
                throw new InvalidOperationException();
        }

        internal class TextContentChangedEventRaiser : ITextEventRaiser
        {
            private readonly TextContentChangedEventArgs _args;

            public bool HasPostEvent => true;

            public TextContentChangedEventRaiser(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot,
                EditOptions options, object editTag)
            {
                _args = new TextContentChangedEventArgs(beforeSnapshot, afterSnapshot, options, editTag);
            }

            public void RaiseEvent(BaseBuffer baseBuffer, bool immediate)
            {
                baseBuffer.RawRaiseEvent(_args, immediate);
            }
        }

        protected class ContentTypeChangedEventRaiser : ITextEventRaiser
        {
            private readonly IContentType _afterContentType;
            private readonly ITextSnapshot _afterSnapshot;
            private readonly IContentType _beforeContentType;
            private readonly ITextSnapshot _beforeSnapshot;
            private readonly object _editTag;

            public bool HasPostEvent => true;

            public ContentTypeChangedEventRaiser(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot,
                IContentType beforeContentType, IContentType afterContentType, object editTag)
            {
                _beforeSnapshot = beforeSnapshot;
                _afterSnapshot = afterSnapshot;
                _editTag = editTag;
                _beforeContentType = beforeContentType;
                _afterContentType = afterContentType;
            }

            public void RaiseEvent(BaseBuffer baseBuffer, bool immediate)
            {
                var eventHandlers = immediate ? baseBuffer.ContentTypeChangedImmediate : baseBuffer.ContentTypeChanged;
                if (eventHandlers == null)
                    return;
                var args = new ContentTypeChangedEventArgs(_beforeSnapshot, _afterSnapshot, _beforeContentType,
                    _afterContentType, _editTag);
                baseBuffer._guardedOperations.RaiseEvent(baseBuffer, eventHandlers, args);
            }
        }

        protected abstract class Edit : TextBufferEdit, ITextEdit
        {
            protected readonly EditOptions Options;
            protected readonly int? ReiteratedVersionNumber;
            protected FrugalList<TextChange> Changes;
            private readonly int _bufferLength;
            private Action _cancelAction;
            private TextContentChangingEventArgs _raisedChangingEventArgs;

            public bool HasEffectiveChanges => Changes.Count > 0;

            public bool HasFailedChanges { get; private set; }

            protected Edit(BaseBuffer baseBuffer, ITextSnapshot originSnapshot, EditOptions options,
                int? reiteratedVersionNumber, object editTag)
                : base(baseBuffer, originSnapshot, editTag)
            {
                _bufferLength = originSnapshot.Length;
                Changes = new FrugalList<TextChange>();
                Options = options;
                ReiteratedVersionNumber = reiteratedVersionNumber;
                _raisedChangingEventArgs = null;
                _cancelAction = null;
                HasFailedChanges = false;
            }

            public override void Cancel()
            {
                base.Cancel();
                _cancelAction?.Invoke();
            }

            public bool CheckForCancellation(Action cancelationResponse)
            {
                if (Changes.Count == 0)
                    return true;
                if (_raisedChangingEventArgs == null)
                {
                    _cancelAction = cancelationResponse;
                    _raisedChangingEventArgs = new TextContentChangingEventArgs(Snapshot, EditTag, args => Cancel());
                    BaseBuffer.RaiseChangingEvent(_raisedChangingEventArgs);
                }

                Canceled = _raisedChangingEventArgs.Canceled;
                return !_raisedChangingEventArgs.Canceled;
            }

            public bool Delete(int startPosition, int charsToDelete)
            {
                CheckActive();
                if (startPosition < 0 || startPosition > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(startPosition));
                if (charsToDelete < 0 || startPosition + charsToDelete > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(charsToDelete));
                if (BaseBuffer.IsReadOnlyImplementation(new Span(startPosition, charsToDelete), true))
                {
                    HasFailedChanges = true;
                    return false;
                }

                if (charsToDelete != 0)
                    Changes.Add(TextChange.Create(startPosition,
                        DeletionChangeString(new Span(startPosition, charsToDelete)), StringRebuilder.Empty,
                        OriginSnapshot));
                return true;
            }

            public bool Delete(Span deleteSpan)
            {
                CheckActive();
                if (deleteSpan.End > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(deleteSpan));
                if (BaseBuffer.IsReadOnlyImplementation(deleteSpan, true))
                {
                    HasFailedChanges = true;
                    return false;
                }

                if (deleteSpan.Length != 0)
                    Changes.Add(TextChange.Create(deleteSpan.Start, DeletionChangeString(deleteSpan),
                        StringRebuilder.Empty, OriginSnapshot));
                return true;
            }

            public bool Insert(int position, string text)
            {
                CheckActive();
                if (position < 0 || position > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(position));
                if (text == null)
                    throw new ArgumentNullException(nameof(text));
                if (BaseBuffer.IsReadOnlyImplementation(position, true))
                {
                    HasFailedChanges = true;
                    return false;
                }

                if (text.Length != 0)
                    Changes.Add(TextChange.Create(position, string.Empty, text, OriginSnapshot));
                return true;
            }

            public bool Insert(int position, char[] characterBuffer, int startIndex, int length)
            {
                CheckActive();
                if (position < 0 || position > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(position));
                if (characterBuffer == null)
                    throw new ArgumentNullException(nameof(characterBuffer));
                if (startIndex < 0 || startIndex > characterBuffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(startIndex));
                if (length < 0 || startIndex + length > characterBuffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(length));
                if (BaseBuffer.IsReadOnlyImplementation(position, true))
                {
                    HasFailedChanges = true;
                    return false;
                }

                if (length != 0)
                    Changes.Add(TextChange.Create(position, string.Empty,
                        new string(characterBuffer, startIndex, length), OriginSnapshot));
                return true;
            }

            public void RecordMasterChangeOffset(int masterChangeOffset)
            {
                if (Changes.Count == 0)
                    throw new InvalidOperationException("Can't record a change offset without a change.");
                Changes[Changes.Count - 1].RecordMasterChangeOffset(masterChangeOffset);
            }

            public bool Replace(int startPosition, int charsToReplace, string replaceWith)
            {
                CheckActive();
                if (startPosition < 0 || startPosition > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(startPosition));
                if (charsToReplace < 0 || startPosition + charsToReplace > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(charsToReplace));
                if (replaceWith == null)
                    throw new ArgumentNullException(nameof(replaceWith));
                if (BaseBuffer.IsReadOnlyImplementation(new Span(startPosition, charsToReplace), true))
                {
                    HasFailedChanges = true;
                    return false;
                }

                if (charsToReplace != 0 || replaceWith.Length != 0)
                    Changes.Add(TextChange.Create(startPosition,
                        DeletionChangeString(new Span(startPosition, charsToReplace)), replaceWith, OriginSnapshot));
                return true;
            }

            public bool Replace(Span replaceSpan, string replaceWith)
            {
                CheckActive();
                if (replaceSpan.End > _bufferLength)
                    throw new ArgumentOutOfRangeException(nameof(replaceSpan));
                if (replaceWith == null)
                    throw new ArgumentNullException(nameof(replaceWith));
                if (BaseBuffer.IsReadOnlyImplementation(replaceSpan, true))
                {
                    HasFailedChanges = true;
                    return false;
                }

                if (replaceSpan.Length != 0 || replaceWith.Length != 0)
                    Changes.Add(TextChange.Create(replaceSpan.Start, DeletionChangeString(replaceSpan), replaceWith,
                        OriginSnapshot));
                return true;
            }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                for (var index = 0; index < Changes.Count; ++index)
                {
                    var change = Changes[index];
                    stringBuilder.Append(change.ToString(true));
                    if (index < Changes.Count - 1)
                        stringBuilder.Append("\r\n");
                }

                return stringBuilder.ToString();
            }

            private StringRebuilder DeletionChangeString(Span deleteSpan)
            {
                return BufferFactoryService.StringRebuilderFromSnapshotAndSpan(OriginSnapshot, deleteSpan);
            }
        }

        protected class ReadOnlyRegionsChangedEventRaiser : ITextEventRaiser
        {
            private readonly SnapshotSpan _affectedSpan;

            public bool HasPostEvent => false;

            public ReadOnlyRegionsChangedEventRaiser(SnapshotSpan affectedSpan)
            {
                _affectedSpan = affectedSpan;
            }

            public void RaiseEvent(BaseBuffer baseBuffer, bool immediate)
            {
                var onlyRegionsChanged = baseBuffer.ReadOnlyRegionsChanged;
                if (onlyRegionsChanged == null)
                    return;
                var args = new SnapshotSpanEventArgs(_affectedSpan);
                baseBuffer._guardedOperations.RaiseEvent(baseBuffer, onlyRegionsChanged, args);
            }
        }

        protected abstract class TextBufferBaseEdit : IDisposable
        {
            protected bool Applied;
            protected BaseBuffer BaseBuffer;

            public bool Canceled { get; protected set; }

            protected TextBufferBaseEdit(BaseBuffer baseBuffer)
            {
                BaseBuffer = baseBuffer;
                if (!baseBuffer.CheckEditAccess())
                    throw new InvalidOperationException();
                if (baseBuffer.EditInProgressProtected)
                    throw new InvalidOperationException();
                baseBuffer.EditInProgressProtected = true;
                baseBuffer.Group.BeginEdit();
            }

            public virtual void Cancel()
            {
                CancelApplication();
            }

            public virtual void CancelApplication()
            {
                if (Canceled)
                    return;
                Canceled = true;
                BaseBuffer.EditInProgressProtected = false;
                BaseBuffer.Group.CancelEdit();
            }

            public void Dispose()
            {
                if (!Applied && !Canceled)
                    CancelApplication();
                GC.SuppressFinalize(this);
            }
        }

        protected abstract class TextBufferEdit : TextBufferBaseEdit
        {
            protected object EditTag;
            protected ITextSnapshot OriginSnapshot;

            public ITextSnapshot Snapshot => OriginSnapshot;

            protected TextBufferEdit(BaseBuffer baseBuffer, ITextSnapshot snapshot, object editTag)
                : base(baseBuffer)
            {
                BaseBuffer = baseBuffer;
                OriginSnapshot = snapshot;
                EditTag = editTag;
            }

            public ITextSnapshot Apply()
            {
                try
                {
                    return PerformApply();
                }
                finally
                {
                    if (!Canceled)
                        BaseBuffer.Group.FinishEdit();
                }
            }

            protected void CheckActive()
            {
                if (Canceled)
                    throw new InvalidOperationException();
                if (Applied)
                    throw new InvalidOperationException();
            }

            protected abstract ITextSnapshot PerformApply();
        }

        private sealed class ContentTypeEdit : TextBufferEdit, ISubordinateTextEdit
        {
            private readonly IContentType _newContentType;

            public ITextBuffer TextBuffer => BaseBuffer;

            public ContentTypeEdit(BaseBuffer baseBuffer, ITextSnapshot originSnapshot, object editTag,
                IContentType newContentType)
                : base(baseBuffer, originSnapshot, editTag)
            {
                _newContentType = newContentType;
            }

            public bool CheckForCancellation(Action cancelAction)
            {
                return true;
            }

            public void FinalApply()
            {
                var contentType = BaseBuffer.ContentType;
                BaseBuffer.ContentType = _newContentType;
                BaseBuffer.SetCurrentVersionAndSnapshot(NormalizedTextChangeCollection.Empty);
                ITextEventRaiser raiser = new ContentTypeChangedEventRaiser(OriginSnapshot,
                    BaseBuffer.CurrentSnapshotProtected, contentType, BaseBuffer.ContentType, EditTag);
                BaseBuffer.Group.EnqueueEvents(raiser, BaseBuffer);
                raiser.RaiseEvent(BaseBuffer, true);
                BaseBuffer.EditInProgressProtected = false;
            }

            public void PreApply()
            {
            }

            public void RecordMasterChangeOffset(int masterChangeOffset)
            {
                throw new InvalidOperationException("Content type edits shouldn't have change offsets.");
            }

            protected override ITextSnapshot PerformApply()
            {
                CheckActive();
                Applied = true;
                if (_newContentType != null)
                    BaseBuffer.Group.PerformMasterEdit(BaseBuffer, this, EditOptions.None, EditTag);
                else
                    BaseBuffer.EditInProgressProtected = false;
                return BaseBuffer.CurrentSnapshotProtected;
            }
        }

        private sealed class ReadOnlyRegionEdit : TextBufferEdit, IReadOnlyRegionEdit
        {
            private readonly List<IReadOnlyRegion> _readOnlyRegionsToAdd = new List<IReadOnlyRegion>();
            private readonly List<IReadOnlyRegion> _readOnlyRegionsToRemove = new List<IReadOnlyRegion>();
            private int _aggregateEnd = int.MinValue;
            private int _aggregateStart = int.MaxValue;

            public ReadOnlyRegionEdit(BaseBuffer baseBuffer, ITextSnapshot originSnapshot, object editTag)
                : base(baseBuffer, originSnapshot, editTag)
            {
            }

            public IReadOnlyRegion CreateDynamicReadOnlyRegion(Span span, SpanTrackingMode trackingMode,
                EdgeInsertionMode edgeInsertionMode, DynamicReadOnlyRegionQuery callback)
            {
                var readOnlyRegion = new ReadOnlyRegion(BaseBuffer.CurrentVersion, span, trackingMode,
                    edgeInsertionMode, callback);
                _readOnlyRegionsToAdd.Add(readOnlyRegion);
                _aggregateStart = Math.Min(_aggregateStart, span.Start);
                _aggregateEnd = Math.Max(_aggregateEnd, span.End);
                return readOnlyRegion;
            }

            public IReadOnlyRegion CreateReadOnlyRegion(Span span, SpanTrackingMode trackingMode,
                EdgeInsertionMode edgeInsertionMode)
            {
                return CreateDynamicReadOnlyRegion(span, trackingMode, edgeInsertionMode, null);
            }

            public IReadOnlyRegion CreateReadOnlyRegion(Span span)
            {
                return CreateReadOnlyRegion(span, SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Allow);
            }

            public void RemoveReadOnlyRegion(IReadOnlyRegion readOnlyRegion)
            {
                if (BaseBuffer._readOnlyRegions == null)
                    throw new InvalidOperationException();
                if (_readOnlyRegionsToRemove.Exists(match => match.Span.TextBuffer != BaseBuffer))
                    throw new InvalidOperationException();
                _readOnlyRegionsToRemove.Add(readOnlyRegion);
                Span span = readOnlyRegion.Span.GetSpan(BaseBuffer.CurrentSnapshot);
                _aggregateStart = Math.Min(_aggregateStart, span.Start);
                _aggregateEnd = Math.Max(_aggregateEnd, span.End);
            }

            protected override ITextSnapshot PerformApply()
            {
                CheckActive();
                Applied = true;
                if (_readOnlyRegionsToAdd.Count > 0 || _readOnlyRegionsToRemove.Count > 0)
                {
                    if (_readOnlyRegionsToAdd.Count > 0)
                    {
                        if (BaseBuffer._readOnlyRegions == null)
                            BaseBuffer._readOnlyRegions = new FrugalList<IReadOnlyRegion>();
                        BaseBuffer._readOnlyRegions.AddRange(_readOnlyRegionsToAdd);
                    }

                    if (_readOnlyRegionsToRemove.Count > 0)
                        foreach (var readOnlyRegion in _readOnlyRegionsToRemove)
                            BaseBuffer._readOnlyRegions.Remove(readOnlyRegion);
                    BaseBuffer.ReadOnlyRegionSpanCollection =
                        new ReadOnlySpanCollection(BaseBuffer.CurrentVersion, BaseBuffer._readOnlyRegions);
                    BaseBuffer.Group.EnqueueEvents(
                        new ReadOnlyRegionsChangedEventRaiser(new SnapshotSpan(BaseBuffer.CurrentSnapshot,
                            _aggregateStart, _aggregateEnd - _aggregateStart)), BaseBuffer);
                    BaseBuffer.EditInProgressProtected = false;
                }
                else
                {
                    BaseBuffer.EditInProgressProtected = false;
                }

                return OriginSnapshot;
            }
        }
    }
}