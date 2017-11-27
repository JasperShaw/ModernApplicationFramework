namespace ModernApplicationFramework.EditorBase.Controls.SaveDirtyDocumentsDialog
{
    public sealed class SaveDirtyDocumentItem
    {
        public SaveDirtyDocumentItem(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}
