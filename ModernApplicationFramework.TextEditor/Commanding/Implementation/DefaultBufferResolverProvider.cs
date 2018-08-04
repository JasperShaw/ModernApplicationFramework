using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Commanding.Implementation
{
    [Export(typeof(ICommandingTextBufferResolverProvider))]
    [ContentType("any")]
    internal class DefaultBufferResolverProvider : ICommandingTextBufferResolverProvider
    {
        public ICommandingTextBufferResolver CreateResolver(ITextView textView)
        {
            return new DefaultBufferResolver(textView);
        }
    }
}