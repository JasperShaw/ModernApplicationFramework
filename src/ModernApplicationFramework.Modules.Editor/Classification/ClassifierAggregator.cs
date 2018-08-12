using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    internal sealed class ClassifierAggregator : IAccurateClassifier, IDisposable
    {
        private readonly IClassificationTypeRegistryService _classificationTypeRegistry;
        private readonly ITextBuffer _textBuffer;
        private IAccurateTagAggregator<IClassificationTag> _tagAggregator;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        internal ClassifierAggregator(ITextBuffer textBuffer,
            IBufferTagAggregatorFactoryService bufferTagAggregatorFactory,
            IClassificationTypeRegistryService classificationTypeRegistry)
        {
            if (bufferTagAggregatorFactory == null)
                throw new ArgumentNullException(nameof(bufferTagAggregatorFactory));
            _textBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));
            _classificationTypeRegistry = classificationTypeRegistry ??
                                          throw new ArgumentNullException(nameof(classificationTypeRegistry));
            _tagAggregator =
                bufferTagAggregatorFactory.CreateTagAggregator<IClassificationTag>(textBuffer,
                    TagAggregatorOptions.MapByContentType) as IAccurateTagAggregator<IClassificationTag>;
            _tagAggregator.BatchedTagsChanged += OnBatchedTagsChanged;
        }

        internal ClassifierAggregator(ITextView textView, IViewTagAggregatorFactoryService viewTagAggregatorFactory,
            IClassificationTypeRegistryService classificationTypeRegistry)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (viewTagAggregatorFactory == null)
                throw new ArgumentNullException(nameof(viewTagAggregatorFactory));
            _textBuffer = textView.TextBuffer;
            _classificationTypeRegistry = classificationTypeRegistry ??
                                          throw new ArgumentNullException(nameof(classificationTypeRegistry));
            _tagAggregator =
                viewTagAggregatorFactory.CreateTagAggregator<IClassificationTag>(textView,
                    TagAggregatorOptions.MapByContentType) as IAccurateTagAggregator<IClassificationTag>;
            _tagAggregator.BatchedTagsChanged += OnBatchedTagsChanged;
        }

        public void Dispose()
        {
            if (_tagAggregator == null)
                return;
            _tagAggregator.BatchedTagsChanged -= OnBatchedTagsChanged;
            _tagAggregator.Dispose();
            _tagAggregator = null;
        }

        public IList<ClassificationSpan> GetAllClassificationSpans(SnapshotSpan span, CancellationToken cancel)
        {
            if (_tagAggregator == null)
                return new List<ClassificationSpan>(0);
            return InternalGetClassificationSpans(span, _tagAggregator.GetAllTags(span, cancel));
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (_tagAggregator == null)
                return new List<ClassificationSpan>(0);
            return InternalGetClassificationSpans(span, _tagAggregator.GetTags(span));
        }

        private void AddClassificationSpan(List<OpenSpanData> openSpans, ITextSnapshot snapshot, int start, int end,
            IList<ClassificationSpan> results)
        {
            IClassificationType classificationType;
            if (openSpans.Count == 1)
            {
                classificationType = openSpans[0].ClassificationType;
            }
            else
            {
                var classificationTypeList = new List<IClassificationType>(openSpans.Count);
                foreach (var openSpan in openSpans)
                    classificationTypeList.Add(openSpan.ClassificationType);
                classificationType =
                    _classificationTypeRegistry.CreateTransientClassificationType(classificationTypeList);
            }

            results.Add(new ClassificationSpan(new SnapshotSpan(snapshot, start, end - start), classificationType));
        }

        private int Compare(PointData a, PointData b)
        {
            if (a.Position == b.Position)
                return b.IsStart.CompareTo(a.IsStart);
            return a.Position - b.Position;
        }

        private IList<ClassificationSpan> InternalGetClassificationSpans(SnapshotSpan span,
            IEnumerable<IMappingTagSpan<IClassificationTag>> tags)
        {
            var classificationSpanList = new List<ClassificationSpan>();
            foreach (var tag in tags)
            {
                var spans = tag.Span.GetSpans(span.Snapshot.TextBuffer);
                for (var index = 0; index < spans.Count; ++index)
                    classificationSpanList.Add(new ClassificationSpan(
                        spans[index].TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive),
                        tag.Tag.ClassificationType));
            }

            return NormalizeClassificationSpans(span, classificationSpanList);
        }

        private List<ClassificationSpan> NormalizeClassificationSpans(SnapshotSpan requestedRange,
            IList<ClassificationSpan> spans)
        {
            var pointDataList = new List<PointData>(spans.Count * 2);
            var num = -1;
            IClassificationType classificationType = null;
            var flag = false;
            foreach (var span1 in spans)
            {
                var nullable1 =
                    requestedRange.Overlap(span1.Span.TranslateTo(requestedRange.Snapshot,
                        SpanTrackingMode.EdgeExclusive));
                Span? nullable2 = nullable1;
                if (nullable2.HasValue)
                {
                    var span2 = nullable2.Value;
                    if (span2.Start < num || span2.Start == num && span1.ClassificationType == classificationType)
                        flag = true;
                    num = span2.End;
                    classificationType = span1.ClassificationType;
                    pointDataList.Add(new PointData(true, span2.Start, classificationType));
                    pointDataList.Add(new PointData(false, span2.End, classificationType));
                }
            }

            var classificationSpanList = new List<ClassificationSpan>();
            if (!flag)
            {
                var index = 1;
                while (index < pointDataList.Count)
                {
                    var pointData1 = pointDataList[index - 1];
                    var pointData2 = pointDataList[index];
                    classificationSpanList.Add(new ClassificationSpan(
                        new SnapshotSpan(requestedRange.Snapshot, pointData1.Position,
                            pointData2.Position - pointData1.Position), pointData1.ClassificationType));
                    index += 2;
                }
            }
            else
            {
                pointDataList.Sort(Compare);
                var start = 0;
                var openSpans = new List<OpenSpanData>();
                foreach (var pointData in pointDataList)
                {
                    OpenSpanData openSpanData = null;
                    foreach (var span in openSpans)
                        if (span.ClassificationType == pointData.ClassificationType)
                        {
                            openSpanData = span;
                            break;
                        }

                    if (pointData.IsStart)
                    {
                        if (openSpanData != null)
                        {
                            ++openSpanData.Count;
                        }
                        else
                        {
                            if (openSpans.Count > 0 && pointData.Position > start)
                                AddClassificationSpan(openSpans, requestedRange.Snapshot, start, pointData.Position,
                                    classificationSpanList);
                            start = pointData.Position;
                            openSpans.Add(new OpenSpanData(pointData.ClassificationType));
                        }
                    }
                    else if (openSpanData.Count > 1)
                    {
                        --openSpanData.Count;
                    }
                    else
                    {
                        if (pointData.Position > start)
                            AddClassificationSpan(openSpans, requestedRange.Snapshot, start, pointData.Position,
                                classificationSpanList);
                        start = pointData.Position;
                        openSpans.Remove(openSpanData);
                    }
                }
            }

            return classificationSpanList;
        }

        private void OnBatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            // ISSUE: reference to a compiler-generated field
            var classificationChanged = ClassificationChanged;
            if (classificationChanged == null)
                return;
            foreach (var span1 in e.Spans)
            foreach (var span2 in span1.GetSpans(_textBuffer))
                classificationChanged(this, new ClassificationChangedEventArgs(span2));
        }

        private struct PointData
        {
            public readonly IClassificationType ClassificationType;
            public readonly bool IsStart;
            public readonly int Position;

            public PointData(bool isStart, int position, IClassificationType classificationType)
            {
                IsStart = isStart;
                Position = position;
                ClassificationType = classificationType;
            }
        }

        private class OpenSpanData
        {
            public readonly IClassificationType ClassificationType;
            public int Count = 1;

            public OpenSpanData(IClassificationType classificationType)
            {
                ClassificationType = classificationType;
            }
        }
    }
}