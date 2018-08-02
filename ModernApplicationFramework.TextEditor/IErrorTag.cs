namespace ModernApplicationFramework.TextEditor
{
    public interface IErrorTag : ITag
    {
        string ErrorType { get; }

        object ToolTipContent { get; }
    }
}