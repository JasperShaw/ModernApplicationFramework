namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface ICommandingTextBufferResolverProvider
    {
        ICommandingTextBufferResolver CreateResolver(ITextView textView);
    }
}