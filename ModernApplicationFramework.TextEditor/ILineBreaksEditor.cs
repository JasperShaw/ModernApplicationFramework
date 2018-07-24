namespace ModernApplicationFramework.TextEditor
{
    public interface ILineBreaksEditor : ILineBreaks
    {
        void Add(int start, int length);
    }
}