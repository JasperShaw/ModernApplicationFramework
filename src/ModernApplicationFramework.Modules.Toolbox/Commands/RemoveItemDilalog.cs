using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    internal static class RemoveItemDilalog
    {
        public static MessageBoxResult AskUserForRemove(IToolboxItem item)
        {
            IoC.Get<IMafUIShell>().GetAppName(out var name);
            return MessageBox.Show("Delete", name, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        }
    }
}
