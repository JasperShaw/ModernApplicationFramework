using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class BufferPositionToTokenIndexMap
    {
        internal IList<MapSegment> Segments;

        public ITextSnapshot SourceTextSnapshot { get; private set; }

        public BufferPositionToTokenIndexMap(ITextSnapshot sourceTextSnapshot, IList<NormalizedSpan> normalizedSpans)
        {
            SourceTextSnapshot = sourceTextSnapshot;
            Segments = new List<MapSegment>();
            var index1 = 0;
            Span span;
            do
            {
                var normalizedSpan = normalizedSpans[index1];
                if (normalizedSpan.ClassifiedRun == null)
                {
                    span = normalizedSpan.BufferSpan;
                    if (span.Length == 0)
                        goto label_4;
                }

                var flag = normalizedSpan.ClassifiedRun == null;
                var startOfRun = FindStartOfRun(normalizedSpans, index1);
                var index2 = flag ? index1 : FindEndOfTextRuns(normalizedSpans, index1);
                var endOfRun = FindEndOfRun(normalizedSpans, index2);
                var segments = Segments;
                span = normalizedSpan.BufferSpan;
                var start1 = span.Start;
                span = normalizedSpans[index2].BufferSpan;
                var end1 = span.End;
                var bufferSpan = Span.FromBounds(start1, end1);
                span = normalizedSpan.TokenSpan;
                var start2 = span.Start;
                span = normalizedSpans[index2].TokenSpan;
                var end2 = span.End;
                var innerTokenSpan = Span.FromBounds(start2, end2);
                span = normalizedSpans[startOfRun].TokenSpan;
                var start3 = span.Start;
                span = normalizedSpans[endOfRun].TokenSpan;
                var end3 = span.End;
                var outerTokenSpan = Span.FromBounds(start3, end3);
                var num = flag ? 1 : 0;
                var mapSegment = new MapSegment(bufferSpan, innerTokenSpan, outerTokenSpan, num != 0);
                segments.Add(mapSegment);
                index1 = endOfRun;
                label_4:
                ++index1;
            } while (index1 < normalizedSpans.Count);

            if (Segments.Count != 0)
                return;
            span = normalizedSpans[normalizedSpans.Count - 1].TokenSpan;
            var start = span.End;
            foreach (var normalizedSpan in normalizedSpans)
                if (normalizedSpan.Affinity == PositionAffinity.Successor)
                {
                    span = normalizedSpan.TokenSpan;
                    start = span.Start;
                    break;
                }

            var segments1 = Segments;
            span = normalizedSpans[0].BufferSpan;
            var mapSegment1 = new MapSegment(new Span(span.Start, 0), new Span(start, 0), new Span(start, 0), false);
            segments1.Add(mapSegment1);
        }

        public int GetAssociatedBufferPositionFromTokenIndex(int tokenIndex)
        {
            return Segments[GetIndexOfSegmentContainingTokenIndex(tokenIndex)]
                .GetAssociatedBufferPositionFromTokenIndex(tokenIndex);
        }

        public Span? GetElisionSpan(int bufferPosition)
        {
            var containingBufferPosition = GetIndexOfSegmentContainingBufferPosition(bufferPosition);
            if (Segments[containingBufferPosition].IsElision)
                return Segments[containingBufferPosition].BufferSpan;
            return new Span?();
        }

        public int GetLeftmostTokenIndexForBufferPosition(int bufferPosition)
        {
            var containingBufferPosition = GetIndexOfSegmentContainingBufferPosition(bufferPosition);
            if (!Segments[containingBufferPosition].IsElision &&
                bufferPosition != Segments[containingBufferPosition].BufferSpan.Start)
                return Segments[containingBufferPosition].GetTokenIndexFromBufferPosition(bufferPosition);
            return Segments[containingBufferPosition].OuterTokenSpan.Start;
        }

        public int GetNextBufferPositionFromTokenIndex(int tokenIndex)
        {
            return Segments[GetIndexOfSegmentContainingTokenIndex(tokenIndex)]
                .GetNextBufferPositionFromTokenIndex(tokenIndex);
        }

        public int GetNextTokenIndex(int tokenIndex, int endTokenIndex, int lineBreakLength)
        {
            var containingTokenIndex = GetIndexOfSegmentContainingTokenIndex(tokenIndex);
            var num = Math.Max(tokenIndex, Segments[containingTokenIndex].InnerTokenSpan.Start) + 1;
            if (num <= Segments[containingTokenIndex].InnerTokenSpan.End)
            {
                if (num < Segments[containingTokenIndex].InnerTokenSpan.End)
                {
                    if (num > endTokenIndex - lineBreakLength)
                        return endTokenIndex;
                    return num;
                }

                if (containingTokenIndex >= Segments.Count - 1)
                    return endTokenIndex;
                return Segments[containingTokenIndex + 1].OuterTokenSpan.Start;
            }

            if (containingTokenIndex < Segments.Count - 1)
                return GetNextTokenIndex(Segments[containingTokenIndex + 1].InnerTokenSpan.Start, endTokenIndex,
                    lineBreakLength);
            return endTokenIndex;
        }

        public int GetRightmostTokenIndexForBufferPosition(int bufferPosition)
        {
            var containingBufferPosition = GetIndexOfSegmentContainingBufferPosition(bufferPosition);
            if (!Segments[containingBufferPosition].IsElision &&
                bufferPosition < Segments[containingBufferPosition].BufferSpan.End - 1)
                return Segments[containingBufferPosition].GetTokenIndexFromBufferPosition(bufferPosition);
            return Segments[containingBufferPosition].OuterTokenSpan.End - 1;
        }

        public int GetTokenIndexFromBufferPosition(int bufferPosition)
        {
            return Segments[GetIndexOfSegmentContainingBufferPosition(bufferPosition)]
                .GetTokenIndexFromBufferPosition(bufferPosition);
        }

        public bool IsTokenIndexABufferPosition(int tokenIndex)
        {
            return Segments[GetIndexOfSegmentContainingTokenIndex(tokenIndex)].IsTokenIndexABufferPosition(tokenIndex);
        }

        public void SetSourceTextSnapshot(ITextSnapshot newSnapshot)
        {
            if (newSnapshot == SourceTextSnapshot)
                return;
            if (AnyChanges(SourceTextSnapshot.Version, newSnapshot.Version))
            {
                var snapshotPoint = new SnapshotPoint(SourceTextSnapshot, Segments[0].BufferSpan.Start);
                int start = snapshotPoint.TranslateTo(newSnapshot, PointTrackingMode.Negative);
                foreach (var segment in Segments)
                {
                    snapshotPoint = new SnapshotPoint(SourceTextSnapshot, segment.BufferSpan.End);
                    int end = snapshotPoint.TranslateTo(newSnapshot, PointTrackingMode.Negative);
                    segment.SetBufferSpan(Span.FromBounds(start, end));
                    start = end;
                }
            }

            SourceTextSnapshot = newSnapshot;
        }

        private static bool AnyChanges(ITextVersion oldVersion, ITextVersion newVersion)
        {
            while (oldVersion.Changes.Count <= 0)
            {
                oldVersion = oldVersion.Next;
                if (oldVersion == newVersion)
                    return false;
            }

            return true;
        }

        private static int FindEndOfRun(IList<NormalizedSpan> normalizedSpans, int index)
        {
            while (++index < normalizedSpans.Count)
            {
                var normalizedSpan = normalizedSpans[index];
                if (normalizedSpan.ClassifiedRun != null || normalizedSpan.Element == null ||
                    normalizedSpan.BufferSpan.Length != 0 || normalizedSpan.Affinity != PositionAffinity.Predecessor)
                    break;
            }

            return index - 1;
        }

        private static int FindEndOfTextRuns(IList<NormalizedSpan> normalizedSpans, int index)
        {
            do
            {
            } while (++index < normalizedSpans.Count && normalizedSpans[index].ClassifiedRun != null);

            return index - 1;
        }

        private static int FindStartOfRun(IList<NormalizedSpan> normalizedSpans, int index)
        {
            while (--index >= 0)
            {
                var normalizedSpan = normalizedSpans[index];
                if (normalizedSpan.Element == null || normalizedSpan.BufferSpan.Length != 0 ||
                    normalizedSpan.Affinity != PositionAffinity.Successor)
                    break;
            }

            return index + 1;
        }

        private int GetIndexOfSegmentContainingBufferPosition(int bufferPosition)
        {
            var num1 = 0;
            var num2 = Segments.Count;
            while (num1 < num2)
            {
                var index = (num1 + num2) / 2;
                if (bufferPosition < Segments[index].BufferSpan.Start)
                    num2 = index;
                else
                    num1 = index + 1;
            }

            return num1 - 1;
        }

        private int GetIndexOfSegmentContainingTokenIndex(int tokenIndex)
        {
            var num1 = 0;
            var num2 = Segments.Count;
            while (num1 < num2)
            {
                var index = (num1 + num2) / 2;
                if (tokenIndex < Segments[index].OuterTokenSpan.Start)
                    num2 = index;
                else
                    num1 = index + 1;
            }

            return Math.Max(0, num1 - 1);
        }

        internal class MapSegment
        {
            public Span BufferSpan;
            public Span InnerTokenSpan;
            public bool IsElision;
            public Span OuterTokenSpan;

            public MapSegment(Span bufferSpan, Span innerTokenSpan, Span outerTokenSpan, bool isElision)
            {
                BufferSpan = bufferSpan;
                InnerTokenSpan = innerTokenSpan;
                OuterTokenSpan = outerTokenSpan;
                IsElision = isElision;
            }

            public int GetAssociatedBufferPositionFromTokenIndex(int tokenIndex)
            {
                if (tokenIndex <= InnerTokenSpan.Start || IsElision)
                    return BufferSpan.Start;
                if (tokenIndex < InnerTokenSpan.End)
                    return BufferSpan.Start + (tokenIndex - InnerTokenSpan.Start);
                return BufferSpan.Start + Math.Max(0, BufferSpan.Length - 1);
            }

            public int GetNextBufferPositionFromTokenIndex(int tokenIndex)
            {
                if (tokenIndex <= InnerTokenSpan.Start)
                    return BufferSpan.Start;
                if (tokenIndex >= InnerTokenSpan.End || IsElision)
                    return BufferSpan.End;
                return BufferSpan.Start + (tokenIndex - InnerTokenSpan.Start);
            }

            public int GetTokenIndexFromBufferPosition(int bufferPosition)
            {
                if (IsElision && bufferPosition < BufferSpan.End)
                    return InnerTokenSpan.Start;
                return InnerTokenSpan.Start + (bufferPosition - BufferSpan.Start);
            }

            public bool IsTokenIndexABufferPosition(int tokenIndex)
            {
                if (!IsElision)
                    return InnerTokenSpan.Contains(tokenIndex);
                return false;
            }

            public void SetBufferSpan(Span bufferSpan)
            {
                BufferSpan = bufferSpan;
            }
        }
    }
}