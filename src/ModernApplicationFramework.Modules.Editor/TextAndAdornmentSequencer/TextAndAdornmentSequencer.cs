using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.TextAndAdornmentSequencer
{
    internal sealed class TextAndAdornmentSequencer : ITextAndAdornmentSequencer
    {
        private readonly ITagAggregator<SpaceNegotiatingAdornmentTag> _tagAggregator;
        private readonly ITextView _view;

        public event EventHandler<TextAndAdornmentSequenceChangedEventArgs> SequenceChanged;

        public IBufferGraph BufferGraph => _view.BufferGraph;

        public ITextBuffer SourceBuffer => _view.TextViewModel.EditBuffer;

        public ITextBuffer TopBuffer => _view.TextViewModel.VisualBuffer;

        private TextAndAdornmentSequencer(ITextView view, IViewTagAggregatorFactoryService tagAggregatorFactoryService)
        {
            _view = view;
            _tagAggregator = tagAggregatorFactoryService.CreateTagAggregator<SpaceNegotiatingAdornmentTag>(view);
            _tagAggregator.TagsChanged += OnTagsChanged;
            var closer = (EventHandler) null;
            closer = (sender, args) =>
            {
                ((ITextView) sender).Closed -= closer;
                _tagAggregator.TagsChanged -= OnTagsChanged;
                _tagAggregator.Dispose();
            };
            _view.Closed += closer;
        }

        public static ITextAndAdornmentSequencer GetSequencer(ITextView view,
            IViewTagAggregatorFactoryService tagAggregatorFactoryService)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (tagAggregatorFactoryService == null)
                throw new ArgumentNullException(nameof(tagAggregatorFactoryService));
            return view.Properties.GetOrCreateSingletonProperty(() =>
                (ITextAndAdornmentSequencer) new TextAndAdornmentSequencer(view, tagAggregatorFactoryService));
        }

        public ITextAndAdornmentCollection CreateTextAndAdornmentCollection(SnapshotSpan topSpan,
            ITextSnapshot sourceTextSnapshot)
        {
            if (sourceTextSnapshot == null)
                throw new ArgumentNullException(nameof(sourceTextSnapshot));
            if (topSpan.Snapshot.TextBuffer != TopBuffer)
                throw new ArgumentException("The source span must be from the visual buffer.", nameof(topSpan));
            if (sourceTextSnapshot.TextBuffer != SourceBuffer)
                throw new ArgumentException("The source line must be from the source buffer.",
                    nameof(sourceTextSnapshot));
            return CreateTextAndAdornmentCollection(
                new SnapshotSpan(
                    MappingHelper.MapDownToBufferNoTrack(topSpan.Start, SourceBuffer, PositionAffinity.Predecessor)
                        .Value,
                    MappingHelper.MapDownToBufferNoTrack(topSpan.End, SourceBuffer, PositionAffinity.Successor).Value),
                MappingHelper.MapDownToBufferNoTrack(topSpan, SourceBuffer, false), sourceTextSnapshot,
                topSpan.Snapshot);
        }

        public ITextAndAdornmentCollection CreateTextAndAdornmentCollection(ITextSnapshotLine topLine,
            ITextSnapshot sourceTextSnapshot)
        {
            if (topLine == null)
                throw new ArgumentNullException(nameof(topLine));
            if (sourceTextSnapshot == null)
                throw new ArgumentNullException(nameof(sourceTextSnapshot));
            if (topLine.Snapshot.TextBuffer != TopBuffer)
                throw new ArgumentException("The source line must be from the visual buffer.", nameof(topLine));
            if (sourceTextSnapshot.TextBuffer != SourceBuffer)
                throw new ArgumentException("The source line must be from the source buffer.",
                    nameof(sourceTextSnapshot));
            var bufferNoTrack =
                MappingHelper.MapDownToBufferNoTrack(topLine.ExtentIncludingLineBreak, SourceBuffer, false);
            return CreateTextAndAdornmentCollection(
                new SnapshotSpan(
                    (int) topLine.Start == 0
                        ? new SnapshotPoint(sourceTextSnapshot, 0)
                        : MappingHelper
                            .MapDownToBufferNoTrack(topLine.Start, SourceBuffer, PositionAffinity.Predecessor).Value,
                    (int) topLine.End == topLine.Snapshot.Length
                        ? new SnapshotPoint(sourceTextSnapshot, sourceTextSnapshot.Length)
                        : MappingHelper.MapDownToBufferNoTrack(topLine.End, SourceBuffer, PositionAffinity.Successor)
                            .Value), bufferNoTrack, sourceTextSnapshot, topLine.Snapshot);
        }

        private static void AddAdornmentIntoSequence(IList<ISequenceElement> elements, AdornmentElement newAdornment)
        {
            var adornmentElement = elements.Count > 0 ? elements[elements.Count - 1] as AdornmentElement : null;
            if (adornmentElement == null || adornmentElement.SourceLocation.End == newAdornment.SourceLocation.Start)
            {
                elements.Add(newAdornment);
            }
            else if (adornmentElement.SourceLocation.Start == newAdornment.SourceLocation.Start)
            {
                if (!(adornmentElement.SourceLocation.End < newAdornment.SourceLocation.End))
                    return;
                elements[elements.Count - 1] = newAdornment;
            }
            else if (adornmentElement.SourceLocation.Start < newAdornment.SourceLocation.Start)
            {
                if (!(adornmentElement.SourceLocation.End < newAdornment.SourceLocation.End))
                    return;
                elements.Add(newAdornment);
                RemoveCaretStopForLastAdornment(elements);
            }
            else
            {
                if (!(adornmentElement.SourceLocation.End < newAdornment.SourceLocation.End))
                    return;
                elements.Add(newAdornment);
                RemoveCaretStopForLastAdornment(elements);
            }
        }

        private static void RemoveCaretStopForLastAdornment(IList<ISequenceElement> elements)
        {
            var index1 = -1;
            for (var index2 = elements.Count - 2; index2 >= 0; --index2)
                if (elements[index2] is AdornmentElement element)
                {
                    if (element.SourceLocation.Length > 0)
                    {
                        index1 = index2;
                        break;
                    }
                }
                else
                {
                    break;
                }

            var element1 = elements[elements.Count - 1] as AdornmentElement;
            if (element1 == null || index1 == -1)
                return;
            var element2 = elements[index1] as AdornmentElement;
            var end = element1.SourceLocation.End;
            elements[index1] = element2.Grow(end);
            for (var index2 = index1 + 1; index2 < elements.Count; ++index2)
                elements[index2] = ((AdornmentElement) elements[index2]).Shrink(end);
        }

        private ITextAndAdornmentCollection CreateTextAndAdornmentCollection(SnapshotSpan tagSource,
            NormalizedSnapshotSpanCollection adornmentSource, ITextSnapshot sourceTextSnapshot,
            ITextSnapshot visualSnapshot)
        {
            var tags = _tagAggregator.GetTags(tagSource);
            var adornmentElementList = new List<AdornmentElement>();
            var lineSpans = new NormalizedSnapshotSpanCollection(tagSource);
            foreach (var tag in tags)
            {
                var adornmentElement = AdornmentElement.Create(visualSnapshot, sourceTextSnapshot, tag, lineSpans);
                if (adornmentElement != null)
                    adornmentElementList.Add(adornmentElement);
            }

            adornmentElementList.Sort(AdornmentElement.Compare);
            IList<ISequenceElement> elements = new List<ISequenceElement>();
            var start = tagSource.Start;
            var index = 0;
            foreach (var newAdornment in adornmentElementList)
            {
                SnapshotSpan snapshotSpan;
                for (;
                    index < adornmentSource.Count &&
                    (int) newAdornment.SourceLocation.Start >= (int) adornmentSource[index].End;
                    ++index)
                {
                    snapshotSpan = adornmentSource[index];
                    if (start < snapshotSpan.Start)
                        start = snapshotSpan.Start;
                    if (!adornmentSource[index].IsEmpty)
                        elements.Add(new TextSpanElement(MappingSpanSnapshot.Create(visualSnapshot,
                            new SnapshotSpan(start, snapshotSpan.End), SpanTrackingMode.EdgeExclusive, BufferGraph)));
                }

                if (index < adornmentSource.Count)
                {
                    snapshotSpan = adornmentSource[index];
                    if (start < snapshotSpan.Start)
                        start = snapshotSpan.Start;
                    if (newAdornment.SourceLocation.Start > start)
                    {
                        elements.Add(new TextSpanElement(MappingSpanSnapshot.Create(visualSnapshot,
                            new SnapshotSpan(start, newAdornment.SourceLocation.Start), SpanTrackingMode.EdgeExclusive,
                            BufferGraph)));
                        start = newAdornment.SourceLocation.Start;
                        if (newAdornment.SourceLocation.Start == snapshotSpan.End)
                            ++index;
                    }
                }

                AddAdornmentIntoSequence(elements, newAdornment);
            }

            for (; index < adornmentSource.Count; ++index)
            {
                if (start < adornmentSource[index].Start)
                    start = adornmentSource[index].Start;
                elements.Add(new TextSpanElement(MappingSpanSnapshot.Create(visualSnapshot,
                    new SnapshotSpan(start, adornmentSource[index].End), SpanTrackingMode.EdgeExclusive, BufferGraph)));
            }

            return new TextAndAdornmentCollection(this, elements);
        }

        private void OnTagsChanged(object sender, TagsChangedEventArgs e)
        {
            var sequenceChanged = SequenceChanged;
            sequenceChanged?.Invoke(sender, new TextAndAdornmentSequenceChangedEventArgs(e.Span));
        }
    }
}