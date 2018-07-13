using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    internal static class ToolboxUserDialogs
    {
        public static MessageBoxResult AskUserForRemove(IToolboxItem item)
        {
            IoC.Get<IMafUIShell>().GetAppName(out var name);
            return MessageBox.Show(string.Format(ToolboxResources.ToolboxItemDeleteMessage, item.Name), name,
                MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        public static MessageBoxResult AskUserForReset()
        {
            IoC.Get<IMafUIShell>().GetAppName(out var name);
            return MessageBox.Show(ToolboxResources.ToolboxResetMessage, name,
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        }
    }
}
