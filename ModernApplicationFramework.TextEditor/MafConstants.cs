using System;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class MafConstants
    {
        public static readonly Guid EditorCommandGroup = new Guid("{1AAC9383-9DCB-487A-9AF7-EEAFD3435B55}");

        public enum EditorCommands : uint
        {
            TypeChar = 1,
            Backspace = 2,
            Left = 7,
            SelectCurrentWord = 90,
            ShowContextMenu = 102,
            OpenUrl = 113,
            DoubleClick = 134,
            LeftClick = 150,
        }
    }
}
