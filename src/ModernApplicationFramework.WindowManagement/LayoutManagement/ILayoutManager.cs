namespace ModernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface ILayoutManager
    {
        int LayoutCount { get; }

        string GetLayoutNameAt(int index);

        string GetLayoutDataAt(int index);

        void ApplyWindowLayout(int index);

        void SaveWindowLayout();

        void ManageWindowLayouts();
    }
}