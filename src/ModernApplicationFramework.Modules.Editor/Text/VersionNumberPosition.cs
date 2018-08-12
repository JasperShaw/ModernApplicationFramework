namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal struct VersionNumberPosition
    {
        public int VersionNumber;
        public int Position;

        public VersionNumberPosition(int versionNumber, int position)
        {
            VersionNumber = versionNumber;
            Position = position;
        }
    }
}