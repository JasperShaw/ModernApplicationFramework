using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;

namespace ModernApplicationFramework.Text
{
    public static class TextBufferOperationHelpers
    {
        public static string GetNewLineCharacterToInsert(ITextSnapshotLine line, IEditorOptions editorOptions)
        {
            var str = (string) null;
            var snapshot = line.Snapshot;
            if (editorOptions.GetReplicateNewLineCharacter())
            {
                if (line.LineBreakLength > 0)
                    str = line.GetLineBreakText();
                else if (snapshot.LineCount > 1)
                    str = snapshot.GetLineFromLineNumber(snapshot.LineCount - 2).GetLineBreakText();
            }

            return str ?? editorOptions.GetNewLineCharacter();
        }

        public static bool HasAnyNonWhitespaceCharacters(ITextSnapshotLine line)
        {
            var line1 = line;
            var snapshotPoint = line.End;
            var position1 = snapshotPoint.Position;
            snapshotPoint = line.Start;
            var position2 = snapshotPoint.Position;
            var startIndex = position1 - position2;
            return line1.IndexOfPreviousNonWhiteSpaceCharacter(startIndex) != -1;
        }

        public static bool TryInsertFinalNewLine(ITextBuffer buffer, IEditorOptions editorOptions)
        {
            var currentSnapshot = buffer.CurrentSnapshot;
            var lineCount = currentSnapshot.LineCount;
            var lineFromLineNumber = currentSnapshot.GetLineFromLineNumber(lineCount - 1);
            var snapshotPoint = lineFromLineNumber.Start;
            var position1 = snapshotPoint.Position;
            snapshotPoint = lineFromLineNumber.EndIncludingLineBreak;
            var position2 = snapshotPoint.Position;
            if (position1 == position2)
                return true;
            ITextSnapshot textSnapshot;
            if ((!editorOptions.IsOptionDefined(DefaultOptions.TrimTrailingWhiteSpaceOptionId, true)
                    ? 0
                    : (!editorOptions.GetOptionValue(DefaultOptions.TrimTrailingWhiteSpaceOptionId) ? 1 : 0)) == 0 &&
                !HasAnyNonWhitespaceCharacters(lineFromLineNumber))
            {
                var includingLineBreak = lineFromLineNumber.ExtentIncludingLineBreak;
                textSnapshot = buffer.Delete(includingLineBreak);
            }
            else
            {
                var characterToInsert = GetNewLineCharacterToInsert(lineFromLineNumber, editorOptions);
                var position3 = lineFromLineNumber.End.Position;
                textSnapshot = buffer.Insert(position3, characterToInsert);
            }

            return textSnapshot != null && currentSnapshot != textSnapshot;
        }
    }
}