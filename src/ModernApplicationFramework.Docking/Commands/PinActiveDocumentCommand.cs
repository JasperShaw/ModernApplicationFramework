using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IPinActiveDocumentCommand))]
    internal class PinActiveDocumentCommand : CommandDefinitionCommand, IPinActiveDocumentCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            Checked = false;
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var result = DockingManager.Instance?.Layout.ActiveContent.Parent is LayoutDocumentPane;
            if (result)
                Checked = DockingManager.Instance.Layout.ActiveContent.IsPinned;
            return result;
        }

        protected override void OnExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc);
            di?.PinCommand.Execute(null);
        }
    }
}
