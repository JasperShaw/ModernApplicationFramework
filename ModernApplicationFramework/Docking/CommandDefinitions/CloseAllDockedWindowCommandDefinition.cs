using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    public sealed class CloseAllDockedWindowCommandDefinition : CommandDefinition
    {
        public CloseAllDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(CloseAllDockedWindows, CanCloseAllDockedWindows);
        }

        private bool CanCloseAllDockedWindows()
        {
            if (DockingManager.Instace == null)
                return false;
            var root = DockingManager.Instace.Layout.ActiveContent?.Root;
            if (root == null)
                return false;

            if (!DockingManager.Instace.CanCloseAll)
                return false;

            return DockingManager.Instace.Layout
                    .Descendents()
                    .OfType<LayoutContent>()
                    .Any(d => d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow);
        }

        private void CloseAllDockedWindows()
        {
            DockingManager.Instace?._ExecuteCloseAllCommand();
        }

        public override ICommand Command { get; }

        public override string Name => "Close All Documents";
        public override string Text => "Close All Documents";
        public override string ToolTip => null;
        public override Uri IconSource => new Uri("/ModernApplicationFramework;component/Resources/Icons/CloseDocumentGroup.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "CloseDocumentGroup";
    }
}
