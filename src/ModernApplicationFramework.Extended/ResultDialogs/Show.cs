using Microsoft.Win32;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.ResultDialogs
{
    public static class Show
    {
        public static ShowCommonDialogResult CommonDialog(CommonDialog commonDialog)
        {
            return new ShowCommonDialogResult(commonDialog);
        }

        public static ShowToolResult<TTool> Tool<TTool>()
            where TTool : ITool
        {
            return new ShowToolResult<TTool>();
        }

        public static ShowToolResult<TTool> Tool<TTool>(TTool tool)
            where TTool : ITool
        {
            return new ShowToolResult<TTool>(tool);
        }

        public static ShowWindowResult<TWindow> Window<TWindow>()
            where TWindow : IWindow
        {
            return new ShowWindowResult<TWindow>();
        }

        public static ShowWindowResult<TWindow> Window<TWindow>(TWindow window)
            where TWindow : IWindow
        {
            return new ShowWindowResult<TWindow>(window);
        }

        public static ShowDialogResult<TWindow> Dialog<TWindow>()
            where TWindow : IWindow
        {
            return new ShowDialogResult<TWindow>();
        }

        public static ShowDialogResult<TWindow> Dialog<TWindow>(TWindow window)
            where TWindow : IWindow
        {
            return new ShowDialogResult<TWindow>(window);
        }
    }
}