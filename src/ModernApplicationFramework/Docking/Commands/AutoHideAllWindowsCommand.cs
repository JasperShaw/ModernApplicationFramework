using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IAutoHideAllWindowsCommand))]
    internal class AutoHideAllWindowsCommand : CommandDefinitionCommand, IAutoHideAllWindowsCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return DockingManager.Instance != null && DockingManager.Instance.Layout.Descendents()
                       .OfType<LayoutAnchorable>().Any(x => !x.IsAutoHidden);
        }

        protected override void OnExecute(object parameter)
        {
            var layoutAnchorables = DockingManager.Instance?.Layout.Descendents().OfType<LayoutAnchorable>();
            if (layoutAnchorables == null)
                return;

            foreach (var anchorable in layoutAnchorables.ToList())
            {
                if (!anchorable.IsAutoHidden)
                    anchorable.ToggleAutoHide();
            }
        }
    }
}