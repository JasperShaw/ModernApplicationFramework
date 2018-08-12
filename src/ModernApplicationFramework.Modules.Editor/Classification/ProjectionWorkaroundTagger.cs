using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    internal class ProjectionWorkaroundTagger : ITagger<ClassificationTag>
    {
        private readonly IDifferenceService _diffService;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private IProjectionBuffer ProjectionBuffer { get; }

        internal ProjectionWorkaroundTagger(IProjectionBuffer projectionBuffer, IDifferenceService diffService)
        {
            ProjectionBuffer = projectionBuffer;
            _diffService = diffService;
            ProjectionBuffer.SourceBuffersChanged += SourceSpansChanged;
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            yield break;
        }

        private static int GetMatchSize(ReadOnlyCollection<SnapshotSpan> spans, Match match)
        {
            var num = 0;
            if (match == null)
                return num;
            var left = match.Left;
            for (var start = left.Start; start < left.End; ++start)
                num += spans[start].Length;
            return num;
        }

        private void RaiseTagsChangedEvent(SnapshotSpan span)
        {
            var tagsChanged = TagsChanged;
            tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
        }

        private void SourceSpansChanged(object sender, ProjectionSourceSpansChangedEventArgs e)
        {
            if (e.Changes.Count != 0)
                return;
            var projectionSpanDifference = ProjectionSpanDiffer.DiffSourceSpans(_diffService, e.Before, e.After);
            var val2 = 0;
            var num1 = int.MaxValue;
            var num2 = int.MinValue;
            foreach (var difference in projectionSpanDifference.DifferenceCollection)
            {
                val2 += GetMatchSize(projectionSpanDifference.DeletedSpans, difference.Before);
                num1 = Math.Min(num1, val2);
                var right = difference.Right;
                var start = right.Start;
                while (true)
                {
                    var num3 = start;
                    right = difference.Right;
                    var end = right.End;
                    if (num3 < end)
                    {
                        val2 += projectionSpanDifference.InsertedSpans[start].Length;
                        ++start;
                    }
                    else
                    {
                        break;
                    }
                }

                num2 = Math.Max(num2, val2);
            }

            if (num1 == int.MaxValue || num2 == int.MinValue)
                return;
            RaiseTagsChangedEvent(new SnapshotSpan(e.After, Span.FromBounds(num1, num2)));
        }
    }
}