namespace ModernApplicationFramework.Modules.Editor.Text
{
    public interface ILineBreaksEditor : ILineBreaks
    {
        void Add(int start, int length);
    }
}