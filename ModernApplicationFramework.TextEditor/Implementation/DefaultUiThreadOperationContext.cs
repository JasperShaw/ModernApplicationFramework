namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class DefaultUiThreadOperationContext : AbstractUiThreadOperationContext
    {
        public DefaultUiThreadOperationContext(bool allowCancellation, string defaultDescription)
            : base(allowCancellation, defaultDescription)
        {
        }
    }
}