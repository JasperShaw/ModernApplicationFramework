namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    public interface IOutputView
    {
        void Clear();
        void ScrollToEnd();
        void AppendText(string text);
        void SetText(string text);
    }
}