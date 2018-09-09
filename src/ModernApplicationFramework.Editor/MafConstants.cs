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
            Tab = 4,
            BackTab = 5,
            Delete = 6,
            Left = 7,
            LeftExt = 8,
            Right = 9,
            RightExt = 10,
            Up = 11,
            UpExt = 12,
            Down = 13,
            DownExt = 14,
            Home = 15,
            HomeExt = 16,
            End = 17,
            EndExt = 18,
            BeginOfLine = 19,
            BeginOfLineExt = 20,
            FirstChar = 21,
            FirstCharExt = 22,
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
