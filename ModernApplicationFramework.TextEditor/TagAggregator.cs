using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class TagAggregator<T> : IAccurateTagAggregator<T> where T : ITag
    {
        internal IDictionary<ITextBuffer, IList<ITagger<T>>> Taggers;
        private readonly TagAggregatorOptions _options;
        private List<Tuple<ITagger<T>, int>> _uniqueTaggers;
        internal ITextView TextView;
        internal MappingSpanLink AcculumatedSpanLinks;
        internal bool Disposed;
        internal bool Initialized;

        internal TagAggregatorFactoryService TagAggregatorFactoryService { get; private set; }

        public TagAggregator(TagAggregatorFactoryService factory, ITextView textView, IBufferGraph bufferGraph, TagAggregatorOptions options)
        {
            TagAggregatorFactoryService = factory;
            TextView = textView;
            BufferGraph = bufferGraph;
            _options = options;
            //this.joinableTaskHelper = new JoinableTaskHelper(factory.JoinableTaskContext);
            if (textView != null)
                textView.Closed += OnTextView_Closed;
            Taggers = new Dictionary<ITextBuffer, IList<ITagger<T>>>();
            _uniqueTaggers = new List<Tuple<ITagger<T>, int>>();
            Initialize();
            BufferGraph.GraphBufferContentTypeChanged += BufferGraph_GraphBufferContentTypeChanged;
            BufferGraph.GraphBuffersChanged += BufferGraph_GraphBuffersChanged;
        }

        private void Initialize()
        {
            if (_options.HasFlag(TagAggregatorOptions.NoProjection))
                Taggers[BufferGraph.TopBuffer] = GatherTaggers(BufferGraph.TopBuffer);
            else
                BufferGraph.GetTextBuffers(buffer =>
                {
                    Taggers[buffer] = GatherTaggers(buffer);
                    return false;
                });
            Initialized = true;
        }

        private void EnsureInitialized()
        {
            if (Disposed || Initialized)
                return;
            Initialize();
            var currentSnapshot = BufferGraph.TopBuffer.CurrentSnapshot;
            RaiseEvents(this, BufferGraph.CreateMappingSpan(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length), SpanTrackingMode.EdgeInclusive));
        }

        public IBufferGraph BufferGraph { get; private set; }

        public IEnumerable<IMappingTagSpan<T>> GetTags(SnapshotSpan span)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(TagAggregator<T>));
            if (_uniqueTaggers.Count == 0)
                return Enumerable.Empty<IMappingTagSpan<T>>();
            return InternalGetTags(new NormalizedSnapshotSpanCollection(span), new CancellationToken?());
        }

        public IEnumerable<IMappingTagSpan<T>> GetTags(IMappingSpan span)
        {
            if (span == null)
                throw new ArgumentNullException(nameof(span));
            if (Disposed)
                throw new ObjectDisposedException(nameof(TagAggregator<T>));
            if (_uniqueTaggers.Count == 0)
                return Enumerable.Empty<IMappingTagSpan<T>>();
            return InternalGetTags(span, new CancellationToken?());
        }

        public IEnumerable<IMappingTagSpan<T>> GetTags(NormalizedSnapshotSpanCollection snapshotSpans)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(TagAggregator<T>));
            if (_uniqueTaggers.Count > 0 && snapshotSpans.Count > 0)
                return InternalGetTags(snapshotSpans, new CancellationToken?());
            return Enumerable.Empty<IMappingTagSpan<T>>();
        }

        public event EventHandler<TagsChangedEventArgs> TagsChanged;

        public event EventHandler<BatchedTagsChangedEventArgs> BatchedTagsChanged;

        public IEnumerable<IMappingTagSpan<T>> GetAllTags(SnapshotSpan span, CancellationToken cancel)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(TagAggregator<T>));
            EnsureInitialized();
            if (_uniqueTaggers.Count == 0)
                return Enumerable.Empty<IMappingTagSpan<T>>();
            return InternalGetTags(new NormalizedSnapshotSpanCollection(span), cancel);
        }

        public IEnumerable<IMappingTagSpan<T>> GetAllTags(IMappingSpan span, CancellationToken cancel)
        {
            if (span == null)
                throw new ArgumentNullException(nameof(span));
            if (Disposed)
                throw new ObjectDisposedException(nameof(TagAggregator<T>));
            EnsureInitialized();
            if (_uniqueTaggers.Count == 0)
                return Enumerable.Empty<IMappingTagSpan<T>>();
            return InternalGetTags(span, cancel);
        }

        public IEnumerable<IMappingTagSpan<T>> GetAllTags(NormalizedSnapshotSpanCollection snapshotSpans, CancellationToken cancel)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(TagAggregator<T>));
            EnsureInitialized();
            if (_uniqueTaggers.Count > 0 && snapshotSpans.Count > 0)
                return InternalGetTags(snapshotSpans, cancel);
            return Enumerable.Empty<IMappingTagSpan<T>>();
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            try
            {
                if (TextView != null)
                    TextView.Closed -= OnTextView_Closed;
                BufferGraph.GraphBufferContentTypeChanged -= BufferGraph_GraphBufferContentTypeChanged;
                BufferGraph.GraphBuffersChanged -= BufferGraph_GraphBuffersChanged;
                DisposeAllTaggers();
            }
            finally
            {
                Taggers = null;
                TagAggregatorFactoryService = null;
                BufferGraph = null;
                TextView = null;
                _uniqueTaggers = null;
                Disposed = true;
            }
        }

        private void SourceTaggerTagsChanged(object sender, SnapshotSpanEventArgs e)
        {
            if (Disposed)
                return;
            var mappingSpan = BufferGraph.CreateMappingSpan(e.Span, SpanTrackingMode.EdgeExclusive);
            RaiseEvents(sender, mappingSpan);
        }

        private void RaiseEvents(object sender, IMappingSpan span)
        {
            // ISSUE: reference to a compiler-generated field
            var tagsChanged = TagsChanged;
            if (tagsChanged != null)
                TagAggregatorFactoryService.GuardedOperations.RaiseEvent(sender, tagsChanged, new TagsChangedEventArgs(span));
            // ISSUE: reference to a compiler-generated field
            if (BatchedTagsChanged == null)
                return;
            var mappingSpanLink1 = Volatile.Read(ref AcculumatedSpanLinks);
            while (true)
            {
                var mappingSpanLink2 = Interlocked.CompareExchange(ref AcculumatedSpanLinks, new MappingSpanLink(mappingSpanLink1, span), mappingSpanLink1);
                if (mappingSpanLink2 != mappingSpanLink1)
                    mappingSpanLink1 = mappingSpanLink2;
                else
                    break;
            }
            if (mappingSpanLink1 != null)
                return;
            Execute.OnUIThread(RaiseBatchedTagsChanged);
        }

        private void RaiseBatchedTagsChanged()
        {
            if (Disposed)
                return;
            var flag = true;
            // ISSUE: reference to a compiler-generated field
            var batchedTagsChanged = BatchedTagsChanged;
            if (batchedTagsChanged != null)
            {
                if (TextView != null)
                {
                    if (TextView.IsClosed)
                        flag = false;
                    else if (TextView.InLayout)
                    {
                        Execute.OnUIThread(RaiseBatchedTagsChanged);
                        return;
                    }
                }
            }
            else
                flag = false;
            var comparand = Volatile.Read(ref AcculumatedSpanLinks);
            while (true)
            {
                var mappingSpanLink = Interlocked.CompareExchange(ref AcculumatedSpanLinks, null, comparand);
                if (mappingSpanLink != comparand)
                    comparand = mappingSpanLink;
                else
                    break;
            }
            if (!flag)
                return;
            var mappingSpanList = new List<IMappingSpan>(comparand.Count);
            do
            {
                mappingSpanList.Add(comparand.Span);
                comparand = comparand.Next;
            }
            while (comparand != null);
            TagAggregatorFactoryService.GuardedOperations.RaiseEvent(this, batchedTagsChanged, new BatchedTagsChangedEventArgs(mappingSpanList));
        }

        private void BufferGraph_GraphBuffersChanged(object sender, GraphBuffersChangedEventArgs e)
        {
            if (Disposed || !Initialized || _options.HasFlag(TagAggregatorOptions.NoProjection))
                return;
            foreach (var removedBuffer in e.RemovedBuffers)
            {
                DisposeAllTaggersOverBuffer(removedBuffer);
                Taggers.Remove(removedBuffer);
            }
            foreach (var addedBuffer in e.AddedBuffers)
                Taggers[addedBuffer] = GatherTaggers(addedBuffer);
        }

        private void BufferGraph_GraphBufferContentTypeChanged(object sender, GraphBufferContentTypeChangedEventArgs e)
        {
            if (Disposed || !Initialized || _options.HasFlag(TagAggregatorOptions.NoProjection) && e.TextBuffer != BufferGraph.TopBuffer)
                return;
            DisposeAllTaggersOverBuffer(e.TextBuffer);
            Taggers[e.TextBuffer] = GatherTaggers(e.TextBuffer);
            var currentSnapshot = e.TextBuffer.CurrentSnapshot;
            RaiseEvents(this, BufferGraph.CreateMappingSpan(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length), SpanTrackingMode.EdgeInclusive));
        }

        private void OnTextView_Closed(object sender, EventArgs args)
        {
            Dispose();
        }

        private IEnumerable<IMappingTagSpan<T>> GetTagsForBuffer(KeyValuePair<ITextBuffer, IList<ITagger<T>>> bufferAndTaggers, NormalizedSnapshotSpanCollection snapshotSpans, ITextSnapshot root, CancellationToken? cancel)
        {
            var snapshot = snapshotSpans[0].Snapshot;
            foreach (var t1 in bufferAndTaggers.Value)
            {
                var tagger = t1;
                IEnumerator<ITagSpan<T>> tags = null;
                try
                {
                    IEnumerable<ITagSpan<T>> tagSpans;
                    if (cancel.HasValue)
                    {
                        cancel.Value.ThrowIfCancellationRequested();
                        tagSpans = !(tagger is IAccurateTagger<T> accurateTagger) ? tagger.GetTags(snapshotSpans) : accurateTagger.GetAllTags(snapshotSpans, cancel.Value);
                    }
                    else
                        tagSpans = tagger.GetTags(snapshotSpans);
                    if (tagSpans != null)
                        tags = tagSpans.GetEnumerator();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    TagAggregatorFactoryService.GuardedOperations.HandleException(tagger, ex);
                }
                if (tags != null)
                {
                    try
                    {
                        while (true)
                        {
                            ITagSpan<T> tagSpan;
                            SnapshotSpan span;
                            do
                            {
                                tagSpan = null;
                                try
                                {
                                    if (tags.MoveNext())
                                        tagSpan = tags.Current;
                                }
                                catch (Exception ex)
                                {
                                    TagAggregatorFactoryService.GuardedOperations.HandleException(tagger, ex);
                                }
                                if (tagSpan != null)
                                    span = tagSpan.Span;
                                else
                                {
                                    goto label_19;
                                }
                            }
                            while (!snapshotSpans.IntersectsWith(span.TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive)));
                            yield return new MappingTagSpan<T>(root == null ? BufferGraph.CreateMappingSpan(span, SpanTrackingMode.EdgeExclusive) : MappingSpanSnapshot.Create(root, span, SpanTrackingMode.EdgeExclusive, BufferGraph), tagSpan.Tag);
                        }
                    }
                    finally
                    {
                        try
                        {
                            tags.Dispose();
                        }
                        catch (Exception ex)
                        {
                            TagAggregatorFactoryService.GuardedOperations.HandleException(tagger, ex);
                        }
                    }
                }
                label_19:
                tagger = null;
                tags = null;
            }
        }

        private IEnumerable<IMappingTagSpan<T>> InternalGetTags(NormalizedSnapshotSpanCollection snapshotSpans, CancellationToken? cancel)
        {
            var targetSnapshot = snapshotSpans[0].Snapshot;
            var mapByContentType = (uint)(_options & TagAggregatorOptions.MapByContentType) > 0U;
            foreach (var tagger in Taggers)
            {
                if (tagger.Value.Count > 0)
                {
                    var frugalList = new FrugalList<SnapshotSpan>();
                    foreach (var t in snapshotSpans)
                        MappingHelper.MapDownToBufferNoTrack(t, tagger.Key, frugalList, mapByContentType);

                    if (frugalList.Count > 0)
                    {
                        var snapshotSpans1 = new NormalizedSnapshotSpanCollection(frugalList);
                        foreach (var mappingTagSpan in GetTagsForBuffer(tagger, snapshotSpans1, targetSnapshot, cancel))
                            yield return mappingTagSpan;
                    }
                }
            }
        }

        private IEnumerable<IMappingTagSpan<T>> InternalGetTags(IMappingSpan mappingSpan, CancellationToken? cancel)
        {
            foreach (var tagger in Taggers)
            {
                if (tagger.Value.Count > 0)
                {
                    var spans = mappingSpan.GetSpans(tagger.Key);
                    if (spans.Count > 0)
                    {
                        foreach (var mappingTagSpan in GetTagsForBuffer(tagger, spans, null, cancel))
                            yield return mappingTagSpan;
                    }
                }
            }
        }

        private void DisposeAllTaggers()
        {
            foreach (var tagger in Taggers)
                DisposeAllTaggersOverBuffer(tagger.Value);
        }

        private void DisposeAllTaggersOverBuffer(ITextBuffer buffer)
        {
            DisposeAllTaggersOverBuffer(Taggers[buffer]);
        }

        private void DisposeAllTaggersOverBuffer(IList<ITagger<T>> taggersOnBuffer)
        {
            foreach (var tagger in taggersOnBuffer)
                UnregisterTagger(tagger);
        }

        internal IList<ITagger<T>> GatherTaggers(ITextBuffer textBuffer)
        {
            var taggerList = new List<ITagger<T>>();
            foreach (var eligibleFactory in TagAggregatorFactoryService.GuardedOperations.FindEligibleFactories(TagAggregatorFactoryService.GetBufferTaggersForType(textBuffer.ContentType, typeof(T)), textBuffer.ContentType, TagAggregatorFactoryService.ContentTypeRegistryService))
            {
                ITaggerProvider taggerProvider = null;
                ITagger<T> tagger = null;
                try
                {
                    taggerProvider = eligibleFactory.Value;
                    tagger = taggerProvider.CreateTagger<T>(textBuffer);
                }
                catch (Exception ex)
                {
                    TagAggregatorFactoryService.GuardedOperations.HandleException(taggerProvider ?? (object)eligibleFactory, ex);
                }
                RegisterTagger(tagger, taggerList);
            }
            if (TextView != null)
            {
                foreach (var eligibleFactory in TagAggregatorFactoryService.GuardedOperations.FindEligibleFactories(TagAggregatorFactoryService.GetViewTaggersForType(textBuffer.ContentType, typeof(T)).Where(f =>
                {
                    if (f.Metadata.TextViewRoles != null)
                        return TextView.Roles.ContainsAny(f.Metadata.TextViewRoles);
                    return true;
                }), textBuffer.ContentType, TagAggregatorFactoryService.ContentTypeRegistryService))
                {
                    IViewTaggerProvider viewTaggerProvider = null;
                    ITagger<T> tagger = null;
                    try
                    {
                        viewTaggerProvider = eligibleFactory.Value;
                        tagger = viewTaggerProvider.CreateTagger<T>(TextView, textBuffer);
                    }
                    catch (Exception ex)
                    {
                        TagAggregatorFactoryService.GuardedOperations.HandleException(viewTaggerProvider ?? (object)eligibleFactory, ex);
                    }
                    RegisterTagger(tagger, taggerList);
                }
            }
            return taggerList;
        }

        private void UnregisterTagger(ITagger<T> tagger)
        {
            var index = _uniqueTaggers.FindIndex(tuple => tuple.Item1 == tagger);
            if (index != -1)
            {
                var uniqueTagger = _uniqueTaggers[index];
                if (uniqueTagger.Item2 == 1)
                {
                    tagger.TagsChanged -= SourceTaggerTagsChanged;
                    _uniqueTaggers.RemoveAt(index);
                }
                else
                    _uniqueTaggers[index] = Tuple.Create(tagger, uniqueTagger.Item2 - 1);
            }
            var disposable = tagger as IDisposable;
            if (disposable == null)
                return;
            TagAggregatorFactoryService.GuardedOperations.CallExtensionPoint(this, () => disposable.Dispose());
        }

        private void RegisterTagger(ITagger<T> tagger, IList<ITagger<T>> newTaggers)
        {
            if (tagger == null)
                return;
            newTaggers.Add(tagger);
            var index = _uniqueTaggers.FindIndex(tuple => tuple.Item1 == tagger);
            if (index == -1)
            {
                tagger.TagsChanged += SourceTaggerTagsChanged;
                _uniqueTaggers.Add(Tuple.Create(tagger, 1));
            }
            else
                _uniqueTaggers[index] = Tuple.Create(tagger, _uniqueTaggers[index].Item2 + 1);
        }

        internal class MappingSpanLink
        {
            public readonly MappingSpanLink Next;
            public readonly IMappingSpan Span;

            public int Count
            {
                get
                {
                    if (Next != null)
                        return Next.Count + 1;
                    return 1;
                }
            }

            public MappingSpanLink(MappingSpanLink next, IMappingSpan span)
            {
                Next = next;
                Span = span;
            }
        }
    }
}