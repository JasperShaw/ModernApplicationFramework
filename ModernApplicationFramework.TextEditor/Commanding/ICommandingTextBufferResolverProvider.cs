namespace ModernApplicationFramework.TextEditor.Commanding
{
    public interface ICommandingTextBufferResolverProvider
    {
        ICommandingTextBufferResolver CreateResolver(ITextView textView);
    }
}