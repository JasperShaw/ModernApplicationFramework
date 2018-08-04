using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Commanding.Implementation
{
    internal class DefaultUiThreadOperationContext : AbstractUiThreadOperationContext
    {
        public DefaultUiThreadOperationContext(bool allowCancellation, string defaultDescription)
            : base(allowCancellation, defaultDescription)
        {
        }
    }
}