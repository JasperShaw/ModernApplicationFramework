namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    public interface IDefaultWindowLayoutProvider
    {
        WindowLayout GetLayout();

        void SetDefaultLayout(string compressedPayload);
    }
}
