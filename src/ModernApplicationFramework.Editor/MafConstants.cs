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
            EndOfLine = 23,
            EndOfLineExt = 24,
            LastChar= 25,
            LastCharExt = 26,
            PageUp = 27,
            PageUpExt = 28,
            PageDown = 29,
            PageDownExt = 30,
            TopLine = 31,
            TopLineExt = 32,
            BottomLine = 33,
            BottomLineExt = 34,
            ScrollUp = 35,
            ScrollDown = 36,
            ScrollPageUp = 37,
            ScrollPageDown = 38,
            ScrollLeft = 39,
            ScrollRight = 40,
            ScrollBottom = 41,
            ScrollCenter = 42,
            ScrollTop = 43,
            SelectAll = 44,
            TabifySelection = 45,
            UntabifySelection = 46,
            MakeLowerCase = 47,
            MakeUpperCase = 48,
            ToggleCase = 49,
            Capitalize = 50,
            SwapAnchor = 51,
            GotoLine = 52,
            GotoBrace = 53,
            GotoBraceExt = 54,
            // GoBack = 55,         OMITTED
            // SelectMode = 56,         OMITTED
            ToggleOverTypeMode = 57,
            Cut = 58,
            Copy = 59,
            Paste = 60,
            CutLine = 61,
            DeleteLine = 62,
            DeleteBlankLines = 63,
            DeleteWhiteSpace = 64,
            DeleteToEndOfLine = 65,
            DeleteToBeginOfLine = 66,
            OpenLineAbove = 67,
            OpenLineBelow = 68,
            Indent = 69,
            Unindent = 70,
            // Omitted Find/Undo/Bookmark
            TransposeChar = 87,
            TransposeWord = 88,
            TransposeLine = 89,
            SelectCurrentWord = 90,
            DeleteWordRight = 91,
            DeleteWordLeft = 92,
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
