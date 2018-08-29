using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IDockAsTabbedDocumentCommand))]
    internal class DockAsTabbedDocumentCommand : CommandDefinitionCommand, IDockAsTabbedDocumentCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            var dc = DockingManager.Instance.Layout.ActiveContent;
            if (dc == null)
                return false;

            var di = DockingManager.Instance.GetLayoutItemFromModel(dc);
            return di?.LayoutElement?.FindParent<LayoutAnchorablePane>() != null;
        }

        protected override void OnExecute(object parameter)
        {
            var dc = DockingManager.Instance.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instance.GetLayoutItemFromModel(dc);
            di?.DockAsDocumentCommand.Execute(null);
        }
    }
}