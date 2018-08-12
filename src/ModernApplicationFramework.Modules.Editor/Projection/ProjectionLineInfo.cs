namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal struct ProjectionLineInfo
    {
        public int LineNumber;
        public int Start;
        public int End;
        public int LineBreakLength;
        public bool StartComplete;
        public bool EndComplete;
    }
}