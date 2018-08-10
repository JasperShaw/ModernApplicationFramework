using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor
{
    internal static class PrimitivesUtilities
    {
        public static int GetColumnOfPoint(ITextSnapshot textSnapshot, SnapshotPoint pointPosition, SnapshotPoint startPosition, int tabSize, Func<SnapshotPoint, SnapshotPoint> getNextPosition)
        {
            var num = 0;
            for (var snapshotPoint = startPosition; snapshotPoint < pointPosition; snapshotPoint = getNextPosition(snapshotPoint))
            {
                if (textSnapshot[snapshotPoint] == '\t')
                    num = (num / tabSize + 1) * tabSize;
                else
                    ++num;
            }
            return num;
        }

        private static bool Edit(ITextBuffer buffer, Func<ITextEdit, bool> editAction)
        {
            using (var edit = buffer.CreateEdit())
            {
                if (!editAction(edit))
                    return false;
                edit.Apply();
                if (edit.Canceled)
                    return false;
            }
            return true;
        }

        public static bool Delete(ITextBuffer buffer, int start, int length)
        {
            return Edit(buffer, edit => edit.Delete(start, length));
        }

        internal static bool Insert(ITextBuffer buffer, int position, string text)
        {
            return Edit(buffer, edit => edit.Insert(position, text));
        }

        internal static bool Delete(ITextBuffer buffer, Span span)
        {
            return Edit(buffer, edit => edit.Delete(span));
        }

        internal static bool Delete(SnapshotSpan span)
        {
            return Edit(span.Snapshot.TextBuffer, edit => edit.Delete(span));
        }

        internal static bool Replace(ITextBuffer buffer, Span span, string replacement)
        {
            return Edit(buffer, edit => edit.Replace(span, replacement));
        }
    }
}