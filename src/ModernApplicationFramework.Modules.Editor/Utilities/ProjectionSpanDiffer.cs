using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal class ProjectionSpanDiffer
    {
        internal List<SnapshotSpan>[] DeletedSurrogates;
        internal List<SnapshotSpan>[] InsertedSurrogates;
        private readonly IDifferenceService _diffService;
        private readonly ReadOnlyCollection<SnapshotSpan> _inputDeletedSnapSpans;
        private readonly ReadOnlyCollection<SnapshotSpan> _inputInsertedSnapSpans;
        private bool _computed;

        public ReadOnlyCollection<SnapshotSpan> DeletedSpans { get; private set; }

        public ReadOnlyCollection<SnapshotSpan> InsertedSpans { get; private set; }

        public ProjectionSpanDiffer(IDifferenceService diffService, ReadOnlyCollection<SnapshotSpan> deletedSnapSpans,
            ReadOnlyCollection<SnapshotSpan> insertedSnapSpans)
        {
            _diffService = diffService;
            _inputDeletedSnapSpans = deletedSnapSpans;
            _inputInsertedSnapSpans = insertedSnapSpans;
        }

        public static ProjectionSpanDifference DiffSourceSpans(IDifferenceService diffService, IProjectionSnapshot left,
            IProjectionSnapshot right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.TextBuffer != right.TextBuffer)
                throw new ArgumentException("left does not belong to the same text buffer as right");
            var projectionSpanDiffer =
                new ProjectionSpanDiffer(diffService, left.GetSourceSpans(), right.GetSourceSpans());
            return new ProjectionSpanDifference(projectionSpanDiffer.GetDifferences(),
                projectionSpanDiffer.InsertedSpans, projectionSpanDiffer.DeletedSpans);
        }

        public IDifferenceCollection<SnapshotSpan> GetDifferences()
        {
            if (!_computed)
            {
                DecomposeSpans();
                _computed = true;
            }

            var snapshotSpanList1 = new List<SnapshotSpan>();
            var snapshotSpanList2 = new List<SnapshotSpan>();
            DeletedSpans = snapshotSpanList1.AsReadOnly();
            InsertedSpans = snapshotSpanList2.AsReadOnly();
            foreach (var t in DeletedSurrogates)
                snapshotSpanList1.AddRange(t);

            foreach (var t in InsertedSurrogates)
                snapshotSpanList2.AddRange(t);

            return _diffService.DifferenceSequences(snapshotSpanList1, snapshotSpanList2);
        }

        internal void DecomposeSpans()
        {
            DeletedSurrogates = new List<SnapshotSpan>[_inputDeletedSnapSpans.Count];
            InsertedSurrogates = new List<SnapshotSpan>[_inputInsertedSnapSpans.Count];
            var dictionary1 = new Dictionary<ITextSnapshot, List<Thing>>();
            for (var position = 0; position < _inputDeletedSnapSpans.Count; ++position)
            {
                var inputDeletedSnapSpan = _inputDeletedSnapSpans[position];
                if (!dictionary1.TryGetValue(inputDeletedSnapSpan.Snapshot, out var thingList))
                {
                    thingList = new List<Thing>();
                    dictionary1.Add(inputDeletedSnapSpan.Snapshot, thingList);
                }

                thingList.Add(new Thing(inputDeletedSnapSpan.Span, position));
                DeletedSurrogates[position] = new List<SnapshotSpan>();
            }

            var dictionary2 = new Dictionary<ITextSnapshot, List<Thing>>();
            for (var position = 0; position < _inputInsertedSnapSpans.Count; ++position)
            {
                var insertedSnapSpan = _inputInsertedSnapSpans[position];
                if (!dictionary2.TryGetValue(insertedSnapSpan.Snapshot, out var thingList))
                {
                    thingList = new List<Thing>();
                    dictionary2.Add(insertedSnapSpan.Snapshot, thingList);
                }

                thingList.Add(new Thing(insertedSnapSpan.Span, position));
                InsertedSurrogates[position] = new List<SnapshotSpan>();
            }

            foreach (var keyValuePair in dictionary1)
            {
                var key = keyValuePair.Key;
                if (dictionary2.TryGetValue(key, out var thingList1))
                {
                    var thingList2 = keyValuePair.Value;
                    thingList1.Sort(Comparison);
                    thingList2.Sort(Comparison);
                    var index1 = 0;
                    var index2 = 0;
                    do
                    {
                        var span1 = thingList1[index1].Span;
                        var span2 = thingList2[index2].Span;
                        var nullable = span1.Overlap(span2);
                        if (!nullable.HasValue)
                        {
                            if (span1.Start < span2.Start)
                                ++index1;
                            else
                                ++index2;
                        }
                        else
                        {
                            var normalizedSpanCollection1 = NormalizedSpanCollection.Difference(
                                new NormalizedSpanCollection(span1), new NormalizedSpanCollection(nullable.Value));
                            Span span3;
                            if (normalizedSpanCollection1.Count > 0)
                            {
                                var position = thingList1[index1].Position;
                                thingList1.RemoveAt(index1);
                                var flag = false;
                                var index3 = 0;
                                while (index3 < normalizedSpanCollection1.Count)
                                {
                                    var span4 = normalizedSpanCollection1[index3];
                                    if (!flag)
                                    {
                                        var start1 = span4.Start;
                                        span3 = nullable.Value;
                                        var start2 = span3.Start;
                                        if (start1 >= start2)
                                        {
                                            thingList1.Insert(index1++, new Thing(nullable.Value, position));
                                            flag = true;
                                            continue;
                                        }
                                    }

                                    thingList1.Insert(index1++, new Thing(span4, position));
                                    ++index3;
                                }

                                if (!flag)
                                    thingList1.Insert(index1++, new Thing(nullable.Value, position));
                                --index1;
                            }

                            var normalizedSpanCollection2 = NormalizedSpanCollection.Difference(
                                new NormalizedSpanCollection(span2), new NormalizedSpanCollection(nullable.Value));
                            if (normalizedSpanCollection2.Count > 0)
                            {
                                var position = thingList2[index2].Position;
                                thingList2.RemoveAt(index2);
                                var flag = false;
                                var index3 = 0;
                                while (index3 < normalizedSpanCollection2.Count)
                                {
                                    var span4 = normalizedSpanCollection2[index3];
                                    if (!flag)
                                    {
                                        var start1 = span4.Start;
                                        span3 = nullable.Value;
                                        var start2 = span3.Start;
                                        if (start1 >= start2)
                                        {
                                            thingList2.Insert(index2++, new Thing(nullable.Value, position));
                                            flag = true;
                                            continue;
                                        }
                                    }

                                    thingList2.Insert(index2++, new Thing(span4, position));
                                    ++index3;
                                }

                                if (!flag)
                                    thingList2.Insert(index2++, new Thing(nullable.Value, position));
                                --index2;
                            }
                        }

                        if (span1.End <= span2.End)
                            ++index1;
                        if (span2.End <= span1.End)
                            ++index2;
                    } while (index1 < thingList1.Count && index2 < thingList2.Count);
                }
            }

            foreach (var keyValuePair in dictionary1)
            foreach (var thing in keyValuePair.Value)
                DeletedSurrogates[thing.Position].Add(new SnapshotSpan(keyValuePair.Key, thing.Span));
            foreach (var keyValuePair in dictionary2)
            foreach (var thing in keyValuePair.Value)
                InsertedSurrogates[thing.Position].Add(new SnapshotSpan(keyValuePair.Key, thing.Span));
        }

        private int Comparison(Thing left, Thing right)
        {
            return left.Span.Start - right.Span.Start;
        }

        private class Thing
        {
            public readonly int Position;
            public readonly Span Span;

            public Thing(Span span, int position)
            {
                Span = span;
                Position = position;
            }
        }
    }
}