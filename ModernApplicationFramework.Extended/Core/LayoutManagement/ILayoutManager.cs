namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    public interface ILayoutManager
    {
        int LayoutCount { get; }

        string GetLayoutNameAt(int index);

        string GetLayoutDataAt(int index);

        void ApplyWindowLayout(int index);

        void SaveWindowLayout();

        void ManageWindowLayouts();
    }
}