using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Modules.Editor.UrlTagger
{
    internal sealed class UrlTagger : ITagger<IUrlTag>
    {
        private readonly int _longLineThreshold;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public UrlTagger(int longLineThreshold)
        {
            _longLineThreshold = longLineThreshold;
        }

        public IEnumerable<ITagSpan<IUrlTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count != 0)
            {
                var snapshotSpan = spans[0];
                var lines = GetIntersectingLines(spans);
                foreach (var t in lines)
                {
                    var line = t;
                    if (line.Length <= _longLineThreshold)
                    {
                        snapshotSpan = line.Extent;
                        var lineText = snapshotSpan.GetText();
                        foreach (var urL in UrlUtility.FindUrLs(lineText, new int?()))
                        {
                            var str1 = lineText;
                            var span = urL.Address;
                            var start1 = span.Start;
                            switch (str1[start1])
                            {
                                case '\t':
                                case ' ':
                                case ':':
                                    continue;
                                default:
                                    var str2 = lineText;
                                    span = urL.Url;
                                    var start2 = span.Start;
                                    span = urL.Url;
                                    var length1 = span.Length;
                                    if (Uri.TryCreate(str2.Substring(start2, length1).Replace("\\\\", "\\"),
                                        UriKind.Absolute, out var result))
                                    {
                                        var tag = new UrlTag(result);
                                        var start3 = line.Start;
                                        span = urL.Url;
                                        var start4 = span.Start;
                                        var start5 = start3 + start4;
                                        span = urL.Url;
                                        var length2 = span.Length;
                                        yield return new TagSpan<UrlTag>(new SnapshotSpan(start5, length2), tag);
                                    }

                                    continue;
                            }
                        }
                    }
                }
            }
        }

        internal static FrugalList<ITextSnapshotLine> GetIntersectingLines(NormalizedSnapshotSpanCollection spans)
        {
            var textSnapshotLine = (ITextSnapshotLine) null;
            var frugalList = new FrugalList<ITextSnapshotLine>();
            foreach (var span in spans)
                if (textSnapshotLine == null || textSnapshotLine.EndIncludingLineBreak < (int) span.End)
                {
                    SnapshotPoint snapshotPoint;
                    if (textSnapshotLine == null || textSnapshotLine.EndIncludingLineBreak <= span.Start)
                    {
                        snapshotPoint = span.Start;
                        textSnapshotLine = snapshotPoint.GetContainingLine();
                        frugalList.Add(textSnapshotLine);
                    }

                    while (textSnapshotLine.EndIncludingLineBreak < span.End &&
                           textSnapshotLine.LineNumber < textSnapshotLine.Snapshot.LineCount)
                    {
                        snapshotPoint = textSnapshotLine.EndIncludingLineBreak;
                        textSnapshotLine = snapshotPoint.GetContainingLine();
                        frugalList.Add(textSnapshotLine);
                    }
                }

            return frugalList;
        }
    }
}