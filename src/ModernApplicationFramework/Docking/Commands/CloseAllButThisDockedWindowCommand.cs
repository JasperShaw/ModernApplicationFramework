using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(ICloseAllButThisDockedWindowCommand))]
    internal class CloseAllButThisDockedWindowCommand : CommandDefinitionCommand, ICloseAllButThisDockedWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            if (DockingManager.Instance == null)
                return false;
            var root = DockingManager.Instance.Layout.ActiveContent?.Root;
            if (root == null)
                return false;

            if (!DockingManager.Instance.Layout.ActiveContent.Root.Manager.CanCloseAllButThis)
                return false;

            return DockingManager.Instance.Layout.ActiveContent.Root.Manager.Layout.Descendents()
                .OfType<LayoutContent>()
                .Any(
                    d =>
                        // I know this does not make much sense but VS behaves like this...
                        //!Equals(d, LayoutElement) &&
                        d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow);
        }

        protected override void OnExecute(object parameter)
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            DockingManager.Instance?._ExecuteCloseAllButThisCommand(dm);
        }
    }
}