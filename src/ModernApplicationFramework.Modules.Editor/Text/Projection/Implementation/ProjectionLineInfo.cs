namespace ModernApplicationFramework.Modules.Editor.Text.Projection.Implementation
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