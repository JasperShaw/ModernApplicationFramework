using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class DialogWindow : WindowBase
    {
        public DialogWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;
            HasDialogFrame = true;
        }
    }
}