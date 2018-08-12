using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.TextEditor
{
    internal static class TextConvert
    {
        public static bool TryToSnapshotSpan(ITextSnapshot snapshot, int startLine, int startIndex, int endLine, int endIndex, out SnapshotSpan result)
        {
            return TryToSnapshotSpan(snapshot, startLine, startIndex, endLine, endIndex, false, out result);
        }

        public static bool TryToSnapshotSpan(ITextSnapshot snapshot, int startLine, int startIndex, int endLine, int endIndex, bool useLengthIncludingLineBreak, out SnapshotSpan result)
        {
            result = new SnapshotSpan();
            if (snapshot == null || startLine < 0 || (startLine >= snapshot.LineCount || endLine < 0) || endLine >= snapshot.LineCount)
                return false;
            ITextSnapshotLine lineFromLineNumber = snapshot.GetLineFromLineNumber(startLine);
            ITextSnapshotLine textSnapshotLine = startLine == endLine ? lineFromLineNumber : snapshot.GetLineFromLineNumber(endLine);
            int num1 = lineFromLineNumber.Length;
            int num2 = textSnapshotLine.Length;
            if (useLengthIncludingLineBreak)
            {
                num1 = lineFromLineNumber.LengthIncludingLineBreak;
                num2 = textSnapshotLine.LengthIncludingLineBreak;
            }
            if (startIndex < 0 || startIndex > num1 || (endIndex < 0 || endIndex > num2))
                return false;
            int start = lineFromLineNumber.Start + startIndex;
            int end = textSnapshotLine.Start + endIndex;
            if (end < start)
                return false;
            result = new SnapshotSpan(snapshot, Span.FromBounds(start, end));
            return true;
        }

        public static bool TryToSnapshotSpan(ITextSnapshot snapshot, TextSpan span, out SnapshotSpan result)
        {
            return TryToSnapshotSpan(snapshot, span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, out result);
        }

        public static bool TryToSnapshotSpan(ITextSnapshot snapshot, TextSpan span, bool useLengthIncludingLineBreak, out SnapshotSpan result)
        {
            return TryToSnapshotSpan(snapshot, span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, useLengthIncludingLineBreak, out result);
        }

        public static bool TryToVirtualSnapshotPoint(ITextSnapshot snapshot, int line, int column, out VirtualSnapshotPoint virtualPoint)
        {
            virtualPoint = new VirtualSnapshotPoint();
            if (line < 0 || line >= snapshot.LineCount || column < 0)
                return false;
            virtualPoint = new VirtualSnapshotPoint(snapshot.GetLineFromLineNumber(line), column);
            return true;
        }

        public static bool TryToVirtualSnapshotSpan(ITextSnapshot snapshot, TextSpan span, out VirtualSnapshotSpan virtualSnapshotSpan)
        {
            virtualSnapshotSpan = new VirtualSnapshotSpan();
            if (snapshot == null || span.iStartLine < 0 || (span.iStartLine >= snapshot.LineCount || span.iEndLine < 0) || (span.iEndLine >= snapshot.LineCount || span.iEndLine < span.iStartLine || (span.iStartIndex < 0 || span.iEndIndex < 0)) || span.iStartLine == span.iEndLine && span.iEndIndex < span.iStartIndex)
                return false;
            virtualSnapshotSpan = new VirtualSnapshotSpan(new VirtualSnapshotPoint(snapshot.GetLineFromLineNumber(span.iStartLine), span.iStartIndex), new VirtualSnapshotPoint(snapshot.GetLineFromLineNumber(span.iEndLine), span.iEndIndex));
            return true;
        }

        public static bool IsValidSpan(ITextSnapshot snapshot, TextSpan span)
        {
            if (span.iStartLine >= 0 && span.iStartLine < snapshot.LineCount && (span.iEndLine >= span.iStartLine && span.iEndLine < snapshot.LineCount))
            {
                ITextSnapshotLine lineFromLineNumber1 = snapshot.GetLineFromLineNumber(span.iStartLine);
                ITextSnapshotLine lineFromLineNumber2 = snapshot.GetLineFromLineNumber(span.iEndLine);
                if (span.iStartIndex >= 0 && span.iStartIndex < lineFromLineNumber1.Length && (span.iEndIndex >= 0 && span.iEndIndex <= lineFromLineNumber2.Length))
                    return true;
            }
            return false;
        }

        public static string ToString(IntPtr pszText, int len)
        {
            if (!(pszText == IntPtr.Zero))
                return Marshal.PtrToStringUni(pszText, len);
            if (len > 0)
                throw new ArgumentOutOfRangeException(nameof(len));
            return "";
        }

        public static TextSpan ToVsTextSpan(ITrackingSpan textSpan, ITextSnapshot snapshot)
        {
            return ToVsTextSpan(textSpan.GetSpan(snapshot));
        }

        public static TextSpan ToVsTextSpan(ITextSnapshot snapshot, Span span)
        {
            ITextSnapshotLine lineFromPosition1 = snapshot.GetLineFromPosition(span.Start);
            ITextSnapshotLine lineFromPosition2 = snapshot.GetLineFromPosition(span.End);
            return new TextSpan()
            {
                iStartLine = lineFromPosition1.LineNumber,
                iStartIndex = span.Start - lineFromPosition1.Start,
                iEndLine = lineFromPosition2.LineNumber,
                iEndIndex = span.End - lineFromPosition2.Start
            };
        }

        public static TextSpan ToVsTextSpan(VirtualSnapshotPoint start, VirtualSnapshotPoint end)
        {
            TextSpan textSpan = new TextSpan();
            int position = end.Position.Position;
            ITextSnapshotLine containingLine = start.Position.GetContainingLine();
            textSpan.iStartLine = containingLine.LineNumber;
            textSpan.iStartIndex = start.Position.Position + start.VirtualSpaces - containingLine.Start;
            ITextSnapshotLine textSnapshotLine = position >= containingLine.End.Position || position < containingLine.Start.Position ? end.Position.GetContainingLine() : containingLine;
            textSpan.iEndLine = textSnapshotLine.LineNumber;
            textSpan.iEndIndex = position + end.VirtualSpaces - textSnapshotLine.Start;
            return textSpan;
        }

        public static TextSpan ToVsTextSpan(VirtualSnapshotSpan virtualSpan)
        {
            return ToVsTextSpan(virtualSpan.Start, virtualSpan.End);
        }

        public static TextSpan ToVsTextSpan(SnapshotSpan span)
        {
            return ToVsTextSpan(span.Snapshot, span.Span);
        }

        public static TextLineChange ToTextLineChange(SnapshotSpan beforeSpan, SnapshotSpan afterSpan)
        {
            TextLineChange textLineChange = new TextLineChange();
            ITextSnapshotLine lineFromPosition1 = beforeSpan.Snapshot.GetLineFromPosition(beforeSpan.Start);
            textLineChange.iStartLine = lineFromPosition1.LineNumber;
            textLineChange.iStartIndex = beforeSpan.Start - lineFromPosition1.Start;
            if (beforeSpan.Length > 0)
            {
                ITextSnapshotLine lineFromPosition2 = beforeSpan.Snapshot.GetLineFromPosition(beforeSpan.End);
                textLineChange.iOldEndLine = lineFromPosition2.LineNumber;
                textLineChange.iOldEndIndex = beforeSpan.End - lineFromPosition2.Start;
            }
            else
            {
                textLineChange.iOldEndLine = textLineChange.iStartLine;
                textLineChange.iOldEndIndex = textLineChange.iStartIndex;
            }
            if (afterSpan.Length > 0)
            {
                ITextSnapshotLine lineFromPosition2 = afterSpan.Snapshot.GetLineFromPosition(afterSpan.End);
                textLineChange.iNewEndLine = lineFromPosition2.LineNumber;
                textLineChange.iNewEndIndex = afterSpan.End - lineFromPosition2.Start;
            }
            else
            {
                textLineChange.iNewEndLine = textLineChange.iStartLine;
                textLineChange.iNewEndIndex = textLineChange.iStartIndex;
            }
            return textLineChange;
        }

        public static Eoltype GetEolType(ITextSnapshotLine line)
        {
            switch (line.LineBreakLength)
            {
                case 0:
                    return Eoltype.EolEof;
                case 1:
                    switch (line.GetLineBreakText()[0])
                    {
                        case '\n':
                            return Eoltype.EolLf;
                        case '\r':
                            return Eoltype.EolCr;
                        case '\x0085':
                            return Eoltype.EolUniLinesep;
                        case '\x2028':
                            return Eoltype.EolUniLinesep;
                        case '\x2029':
                            return Eoltype.EolUniParasep;
                    }
                    break;
                case 2:
                    if (line.GetLineBreakText() == "\r\n")
                        return Eoltype.EolCrlf;
                    break;
            }
            return Eoltype.EolNone;
        }

        public static string GetLineBreak(Eoltype2 eol)
        {
            switch (eol)
            {
                case 0:
                    return "\r\n";
                case (Eoltype2)1:
                    return "\r";
                case (Eoltype2)2:
                    return "\n";
                case (Eoltype2)3:
                    return "\x2028";
                case (Eoltype2)4:
                    return "\x2029";
                case Eoltype2.EolUniNel:
                    return "\x0085";
                default:
                    return "";
            }
        }

        public static bool TryGetLineColFromPosition(ITextSnapshot snapshot, int index, out int line, out int col)
        {
            line = -1;
            col = -1;
            if (index < 0 || index > snapshot.Length)
                return false;
            ITextSnapshotLine lineFromPosition = snapshot.GetLineFromPosition(index);
            line = lineFromPosition.LineNumber;
            col = index - lineFromPosition.Start;
            return true;
        }
    }
}