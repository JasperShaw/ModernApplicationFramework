using System;

namespace ModernApplicationFramework.TextEditor
{
    public struct ChangeInput
    {
        public TextSpan MDelSpan;
        public int MiOldLen;
        public int MiNewLen;
        public IntPtr MPszNewText;
        public uint MDwFlags;
    }
}