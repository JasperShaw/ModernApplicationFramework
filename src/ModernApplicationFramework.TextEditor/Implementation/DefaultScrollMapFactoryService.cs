using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(IScrollMapFactoryService))]
    internal sealed class DefaultScrollMapFactoryService : IScrollMapFactoryService
    {
        public IScrollMap Create(ITextView textView)
        {
            return Create(textView, false);
        }

        public IScrollMap Create(ITextView textView, bool areElisionsExpanded)
        {
            return new DefaultScrollMap(textView, areElisionsExpanded);
        }
    }
}