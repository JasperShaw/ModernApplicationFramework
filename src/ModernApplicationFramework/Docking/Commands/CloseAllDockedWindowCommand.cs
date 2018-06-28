using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(ICloseAllDockedWindowCommand))]
    internal class CloseAllDockedWindowCommand : CommandDefinitionCommand, ICloseAllDockedWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            if (DockingManager.Instance == null)
                return false;
            var root = DockingManager.Instance.Layout.ActiveContent?.Root;
            if (root == null)
                return false;

            if (!DockingManager.Instance.CanCloseAll)
                return false;

            return DockingManager.Instance.Layout
                .Descendents()
                .OfType<LayoutContent>()
                .Any(d => d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow);
        }

        protected override void OnExecute(object parameter)
        {
            DockingManager.Instance?._ExecuteCloseAllCommand();
        }
    }
}