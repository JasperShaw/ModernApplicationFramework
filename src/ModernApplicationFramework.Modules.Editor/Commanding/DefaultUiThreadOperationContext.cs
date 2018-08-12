using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    internal class DefaultUiThreadOperationContext : AbstractUiThreadOperationContext
    {
        public DefaultUiThreadOperationContext(bool allowCancellation, string defaultDescription)
            : base(allowCancellation, defaultDescription)
        {
        }
    }
}