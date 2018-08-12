namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal struct VersionNumberPosition
    {
        public int Position;
        public int VersionNumber;

        public VersionNumberPosition(int versionNumber, int position)
        {
            VersionNumber = versionNumber;
            Position = position;
        }
    }
}