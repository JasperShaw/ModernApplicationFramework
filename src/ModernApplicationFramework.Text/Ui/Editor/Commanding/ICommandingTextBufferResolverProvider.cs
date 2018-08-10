namespace ModernApplicationFramework.Text.Ui.Editor.Commanding
{
    public interface ICommandingTextBufferResolverProvider
    {
        ICommandingTextBufferResolver CreateResolver(ITextView textView);
    }
}