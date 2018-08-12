namespace ModernApplicationFramework.Modules.Editor.Text.Implementation
{
    public interface ILineBreaksEditor : ILineBreaks
    {
        void Add(int start, int length);
    }
}