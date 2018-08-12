using System;

namespace ModernApplicationFramework.Editor
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