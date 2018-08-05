namespace ModernApplicationFramework.TextEditor.Text.Document
{
    public class ChangeTag : ITag
    {
        public ChangeTypes ChangeTypes { get; }

        public ChangeTag(ChangeTypes type)
        {
            ChangeTypes = type;
        }
    }
}