using System;

namespace ModernApplicationFramework.Editor
{
    internal sealed class MafConstants
    {
        public static readonly Guid EditorCommandGroup = new Guid("{1AAC9383-9DCB-487A-9AF7-EEAFD3435B55}");

        public enum EditorCommands : uint
        {
            //Update SimpleTextViewWindow. IsEditingCommand IsSearchingCommand as required
            // Update InnerExec and InnerQueryStatus also
            TypeChar = 1,
            Backspace = 2,
            Return = 3,
            Left = 7,
            Copy = 59,
            SelectCurrentWord = 90,
            ShowContextMenu = 102,
            OpenUrl = 113,
            DoubleClick = 134,
            LeftClick = 150,

            // #12557 is the last used "original 2k command id"
            // Anything above this number is a merge of the newer additions 

            // Intentional small gap
            MoveSelLinesUp = 12600,
            MoveSelLinesDown = 12601,
            ZoomIn = 12602,
            ZoomOut = 12603
        }
    }
}
