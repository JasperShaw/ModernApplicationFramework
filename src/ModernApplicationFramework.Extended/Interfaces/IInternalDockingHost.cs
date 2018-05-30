namespace ModernApplicationFramework.Extended.Interfaces
{
    internal interface IInternalDockingHost : IDockingHost
    {
        bool RaiseDocumentClosing(ILayoutItem layoutItem);
    }
}