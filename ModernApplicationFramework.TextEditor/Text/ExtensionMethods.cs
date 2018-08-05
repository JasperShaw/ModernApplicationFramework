using System;

namespace ModernApplicationFramework.TextEditor.Text
{
    public static class ExtensionMethods
    {
        public static int IndexOfNextNonWhiteSpaceCharacter(this ITextSnapshotLine line, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("start");
            var snapshot = line.Snapshot;
            var position = line.Start.Position;
            for (var index = startIndex; index < line.Length; ++index)
            {
                if (!char.IsWhiteSpace(snapshot[position + index]))
                    return index;
            }
            return -1;
        }

        public static int IndexOfPreviousNonWhiteSpaceCharacter(this ITextSnapshotLine line, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            var snapshot = line.Snapshot;
            var position = line.Start.Position;
            for (var index = startIndex - 1; index >= 0; --index)
            {
                if (!char.IsWhiteSpace(snapshot[position + index]))
                    return index;
            }
            return -1;
        }
    }
}
