using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal static class NormalizedSpanGenerator
    {
        internal static TextEndOfSegment EndSegment = new TextEndOfSegment(1);

        public static IList<NormalizedSpan> Create(ITextAndAdornmentCollection elements, ITextSnapshot sourceSnapshot,
            IAccurateClassifier classifier, IClassificationFormatMap classificationFormatMap,
            CancellationToken? cancel = null)
        {
            IList<NormalizedSpan> normalizedSpanList1 = new List<NormalizedSpan>();
            var offset = 0;
            foreach (var element1 in elements)
            {
                var span1 = element1.Span.GetSpans(sourceSnapshot)[0];
                if (element1.ShouldRenderText)
                {
                    var classifiedRuns = CreateClassifiedRuns(span1, classifier, classificationFormatMap, cancel);
                    foreach (var classifiedRun in classifiedRuns)
                    {
                        Span span2;
                        if (classifiedRun.ContainsBidi)
                        {
                            var normalizedSpanList2 = normalizedSpanList1;
                            var formattingModifier = new TextFormattingModifier(classifiedRun.Properties);
                            var num = 1;
                            span2 = classifiedRun.Span;
                            var bufferSpan = new Span(span2.Start, 0);
                            ref var local = ref offset;
                            var normalizedSpan = new NormalizedSpan(formattingModifier, (PositionAffinity) num,
                                bufferSpan, ref local);
                            normalizedSpanList2.Add(normalizedSpan);
                        }

                        normalizedSpanList1.Add(new NormalizedSpan(classifiedRun, ref offset));
                        if (classifiedRun.ContainsBidi)
                        {
                            var normalizedSpanList2 = normalizedSpanList1;
                            var endSegment = EndSegment;
                            var num = 0;
                            span2 = classifiedRun.Span;
                            var bufferSpan = new Span(span2.End, 0);
                            ref var local = ref offset;
                            var normalizedSpan = new NormalizedSpan(endSegment, (PositionAffinity) num, bufferSpan,
                                ref local);
                            normalizedSpanList2.Add(normalizedSpan);
                        }
                    }
                }
                else
                {
                    if (element1 is IAdornmentElement element2)
                        normalizedSpanList1.Add(new NormalizedSpan(element2,
                            classificationFormatMap.DefaultTextProperties, span1, ref offset));
                }
            }

            if (normalizedSpanList1[normalizedSpanList1.Count - 1].GlyphOrFormattingRun == EndSegment)
                normalizedSpanList1.RemoveAt(normalizedSpanList1.Count - 1);
            return normalizedSpanList1;
        }

        private static void AddRuns(ITextSnapshot snapshot, IList<ClassifiedRun> runs,
            IList<ClassificationSpan> classifications, IClassificationFormatMap classificationFormatMap, int spanStart,
            int spanEnd, ref int startClassificationIndex)
        {
            var rawText = snapshot.GetText(spanStart, spanEnd - spanStart).Replace('\f', ' ').Replace('\v', ' ');
            int end;
            for (var start = spanStart; start < spanEnd; start = end)
            {
                TextRunProperties properties = classificationFormatMap.DefaultTextProperties;
                end = spanEnd;
                if (startClassificationIndex < classifications.Count)
                {
                    var classification = classifications[startClassificationIndex];
                    var num = start;
                    var snapshotPoint = classification.Span.Start;
                    var position = snapshotPoint.Position;
                    if (num < position)
                    {
                        snapshotPoint = classification.Span.Start;
                        end = Math.Min(snapshotPoint.Position, spanEnd);
                    }
                    else
                    {
                        properties = classificationFormatMap.GetTextProperties(classification.ClassificationType);
                        snapshotPoint = classification.Span.End;
                        if (snapshotPoint.Position <= spanEnd)
                        {
                            ++startClassificationIndex;
                            snapshotPoint = classification.Span.End;
                            end = snapshotPoint.Position;
                        }
                    }
                }

                var other = new ClassifiedRun(Span.FromBounds(start, end), properties, rawText, start - spanStart);
                if (runs.Count == 0 || !runs[runs.Count - 1].Merge(other))
                    runs.Add(other);
            }
        }

        private static IList<ClassifiedRun> CreateClassifiedRuns(SnapshotSpan span, IAccurateClassifier classifier,
            IClassificationFormatMap classificationFormatMap, CancellationToken? cancel)
        {
            IList<ClassifiedRun> runs = new List<ClassifiedRun>();
            if (span.Length == 0)
            {
                runs.Add(new ClassifiedRun(span.Span, classificationFormatMap.DefaultTextProperties, string.Empty, 0));
            }
            else
            {
                IList<ClassificationSpan> classificationSpanList;
                try
                {
                    classificationSpanList = cancel.HasValue
                        ? classifier.GetAllClassificationSpans(span, cancel.Value)
                        : classifier.GetClassificationSpans(span);
                }
                catch (OperationCanceledException)
                {
                    classificationSpanList = new List<ClassificationSpan>(0);
                }

                int spanStart = span.Start;
                var startClassificationIndex = 0;
                int end1 = span.End;
                var classifications = classificationSpanList;
                if (classifications.Count > 0)
                {
                    var containingLine = span.Start.GetContainingLine();
                    if (containingLine.LineBreakLength == 2 && end1 == containingLine.EndIncludingLineBreak)
                    {
                        int num = containingLine.End + 1;
                        for (var index = classifications.Count - 1; index >= 0; --index)
                        {
                            var classificationSpan = classifications[index];
                            if (classificationSpan.Span.End > (int) containingLine.End)
                            {
                                int start = classificationSpan.Span.Start;
                                int end2 = classificationSpan.Span.End;
                                if (start == num || end2 == num)
                                {
                                    if (classifications == classificationSpanList)
                                        classifications = new List<ClassificationSpan>(classificationSpanList);
                                    if (start == num)
                                        start = containingLine.End;
                                    if (end2 == num)
                                        end2 = containingLine.End;
                                    if (start == end2)
                                        classifications.RemoveAt(index);
                                    else
                                        classifications[index] = new ClassificationSpan(
                                            new SnapshotSpan(span.Snapshot, start, end2 - start),
                                            classifications[index].ClassificationType);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                var num1 = spanStart;
                var index1 = 0;
                while (end1 - spanStart > NormalizedSpan.MaxTextLineLength)
                {
                    var spanEnd = end1;
                    var flag = false;
                    if (index1 < classifications.Count)
                    {
                        var classificationSpan = classifications[index1];
                        var num2 = num1;
                        var span1 = classificationSpan.Span;
                        int start = span1.Start;
                        if (num2 < start)
                        {
                            span1 = classificationSpan.Span;
                            spanEnd = span1.Start;
                        }
                        else
                        {
                            span1 = classificationSpan.Span;
                            spanEnd = span1.End;
                            flag = true;
                        }
                    }

                    if (spanEnd > spanStart + NormalizedSpan.MaxTextLineLength)
                    {
                        if (num1 > spanStart)
                        {
                            spanEnd = num1;
                        }
                        else
                        {
                            var num2 = 1 + (spanEnd - spanStart - 1) / NormalizedSpan.MaxTextLineLength;
                            var num3 = 1 + (spanEnd - spanStart - 1) / num2;
                            spanEnd = spanStart + num3;
                        }

                        AddRuns(span.Snapshot, runs, classifications, classificationFormatMap, spanStart, spanEnd,
                            ref startClassificationIndex);
                        spanStart = spanEnd;
                    }
                    else if (flag)
                    {
                        ++index1;
                    }

                    num1 = spanEnd;
                }

                AddRuns(span.Snapshot, runs, classifications, classificationFormatMap, spanStart, end1,
                    ref startClassificationIndex);
            }

            return runs;
        }

        internal class TextFormattingModifier : TextModifier
        {
            private readonly TextRunProperties _properties;

            public override FlowDirection FlowDirection => FlowDirection.LeftToRight;

            public override bool HasDirectionalEmbedding => true;

            public override int Length => 1;

            public override TextRunProperties Properties => _properties;

            public TextFormattingModifier(TextRunProperties properties)
            {
                _properties = properties;
            }

            public override TextRunProperties ModifyProperties(TextRunProperties properties)
            {
                return _properties;
            }
        }
    }
}