using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseAllDockedWindowCommandDefinition))]
    public sealed class CloseAllDockedWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => Text;
        public override string Text => DockingResources.CloseAllDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override Uri IconSource => new Uri(
            "/ModernApplicationFramework;component/Resources/Icons/CloseDocumentGroup.xaml",
            UriKind.RelativeOrAbsolute);

        public override string IconId => "CloseDocumentGroup";

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

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
    }
}