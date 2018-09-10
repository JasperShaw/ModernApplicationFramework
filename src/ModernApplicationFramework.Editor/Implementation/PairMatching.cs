using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal static class PairMatching
    {
        private static string OpenChars = "(<[{«༺༼‘“‹⁅⁽₍〈〈《「『【〔〖﴾︵︷︹︻︽︿﹁﹃﹙﹛﹝（［｛｢";
        private static string CloseChars = ")>]}»༻༽’”›⁆⁾₎〉〉》」』】〕〗﴿︶︸︺︼︾﹀﹂﹄﹚﹜﹞）］｝｣";

        private static bool TryGetOpenCharIndex(char openChar, out int index)
        {
            index = OpenChars.IndexOf(openChar);
            return index != -1;
        }

        private static bool TryGetCloseCharIndex(char closeChar, out int index)
        {
            index = CloseChars.IndexOf(closeChar);
            return index != -1;
        }

        private static bool FindSimpleEnclosingPair(SnapshotPoint pointToEnclose, char pairChar, out SnapshotSpan simplePairSpan)
        {
            simplePairSpan = new SnapshotSpan(pointToEnclose, pointToEnclose);
            var containingLine = pointToEnclose.GetContainingLine();
            var text = containingLine.GetText();
            var num1 = pointToEnclose - containingLine.Start;
            var num2 = -1;
            int num3;
            for (var startIndex = 0; -1 != (num3 = text.IndexOf(pairChar, startIndex)); startIndex = num3 + 1)
            {
                if (num2 == -1)
                {
                    num2 = num3;
                    if (num2 > num1)
                        return false;
                }
                else
                {
                    if (num3 >= num1)
                    {
                        simplePairSpan = new SnapshotSpan(containingLine.Start + num2, containingLine.Start + num3);
                        return true;
                    }
                    num2 = -1;
                }
            }
            return false;
        }

        private static bool FindMatchingOpenChar(SnapshotPoint startPoint, char open, char close, int maxLines, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);
            var textSnapshotLine = startPoint.GetContainingLine();
            var text = textSnapshotLine.GetText();
            var lineNumber = textSnapshotLine.LineNumber;
            var index = startPoint - textSnapshotLine.Start;
            if (index >= textSnapshotLine.Length)
                index = textSnapshotLine.Length - 1;
            var val1 = 0;
            if (maxLines > 0)
                val1 = Math.Max(val1, lineNumber - maxLines);
            var num = 0;
            while (true)
            {
                for (; index >= 0; --index)
                {
                    var ch = text[index];
                    if (ch == open)
                    {
                        if (num > 0)
                        {
                            --num;
                        }
                        else
                        {
                            pairSpan = new SnapshotSpan(textSnapshotLine.Start + index, startPoint + 1);
                            return true;
                        }
                    }
                    else if (ch == close)
                        ++num;
                }
                if (--lineNumber >= val1)
                {
                    textSnapshotLine = textSnapshotLine.Snapshot.GetLineFromLineNumber(lineNumber);
                    text = textSnapshotLine.GetText();
                    index = textSnapshotLine.Length - 1;
                }
                else
                    break;
            }
            return false;
        }

        private static bool FindMatchingCloseChar(SnapshotPoint startPoint, char open, char close, int maxLines, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);
            var textSnapshotLine = startPoint.GetContainingLine();
            var text = textSnapshotLine.GetText();
            var lineNumber = textSnapshotLine.LineNumber;
            var index = startPoint - textSnapshotLine.Start;
            var val1 = startPoint.Snapshot.LineCount - 1;
            if (maxLines > 0)
                val1 = Math.Min(val1, lineNumber + maxLines);
            var num = 0;
            while (true)
            {
                for (; index < textSnapshotLine.Length; ++index)
                {
                    var ch = text[index];
                    if (ch == close)
                    {
                        if (num > 0)
                        {
                            --num;
                        }
                        else
                        {
                            pairSpan = new SnapshotSpan(startPoint - 1, textSnapshotLine.Start + index);
                            return true;
                        }
                    }
                    else if (ch == open)
                        ++num;
                }
                if (++lineNumber <= val1)
                {
                    textSnapshotLine = textSnapshotLine.Snapshot.GetLineFromLineNumber(lineNumber);
                    text = textSnapshotLine.GetText();
                    index = 0;
                }
                else
                    break;
            }
            return false;
        }

        private static bool FindAnyEnclosingPairSpan(SnapshotPoint startPoint, int maxLines, out SnapshotSpan enclosingSpan)
        {
            enclosingSpan = new SnapshotSpan(startPoint, startPoint);
            var textSnapshotLine = startPoint.GetContainingLine();
            var text = textSnapshotLine.GetText();
            var lineNumber = textSnapshotLine.LineNumber;
            var index1 = startPoint - textSnapshotLine.Start;
            if (index1 >= textSnapshotLine.Length)
                index1 = textSnapshotLine.Length - 1;
            var val1 = 0;
            if (maxLines > 0)
                val1 = Math.Max(val1, lineNumber - maxLines);
            var numArray = new int[CloseChars.Length];
            while (true)
            {
                for (; index1 >= 0; --index1)
                {
                    var ch = text[index1];
                    if (TryGetOpenCharIndex(ch, out var index2))
                    {
                        if (numArray[index2] == 0)
                        {
                            if (textSnapshotLine.Start.Position + index1 < textSnapshotLine.Snapshot.Length && FindMatchingCloseChar(textSnapshotLine.Start + index1 + 1, OpenChars[index2], CloseChars[index2], maxLines, out enclosingSpan))
                                return true;
                        }
                        else
                            --numArray[index2];
                    }
                    else if (TryGetCloseCharIndex(ch, out index2))
                        ++numArray[index2];
                }
                if (--lineNumber >= val1)
                {
                    textSnapshotLine = textSnapshotLine.Snapshot.GetLineFromLineNumber(lineNumber);
                    text = textSnapshotLine.GetText();
                    index1 = textSnapshotLine.Length - 1;
                }
                else
                    break;
            }
            return false;
        }

        internal static bool FindEnclosingPair(SnapshotPoint point, int maxLines, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(point, point);
            var containingLine = point.GetContainingLine();
            var text = containingLine.GetText();
            var lineNumber = containingLine.LineNumber;
            var index1 = point - containingLine.Start;
            var val1 = 0;
            if (maxLines > 0)
                Math.Max(val1, lineNumber - maxLines);
            var ch = text[index1];
            switch (ch)
            {
                case '"':
                case '\'':
                    return FindSimpleEnclosingPair(point, text[index1], out pairSpan);
                default:
                    if (TryGetOpenCharIndex(ch, out var index2))
                    {
                        return point.Position < point.Snapshot.Length && FindMatchingCloseChar(point + 1, OpenChars[index2], CloseChars[index2], maxLines, out pairSpan);
                    }
                    if (TryGetCloseCharIndex(ch, out index2))
                    {
                        return point.Position > 0 && FindMatchingOpenChar(point - 1, OpenChars[index2], CloseChars[index2], maxLines, out pairSpan);
                    }
                    if (!FindSimpleEnclosingPair(point, '"', out pairSpan) && !FindSimpleEnclosingPair(point, '\'', out pairSpan))
                        return FindAnyEnclosingPairSpan(point, maxLines, out pairSpan);
                    return true;
            }
        }
    }
}