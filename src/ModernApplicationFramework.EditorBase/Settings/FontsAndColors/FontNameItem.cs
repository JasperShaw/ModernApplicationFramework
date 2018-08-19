namespace ModernApplicationFramework.EditorBase.Settings.FontsAndColors
{
    internal struct FontNameItem
    {
        public string Name { get; }

        public bool IsMonospace { get; }

        public FontNameItem(string name, bool isMonospace)
        {
            Name = name;
            IsMonospace = isMonospace;
        }
    }
}