using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Commanding
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