using System;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseAllDockedWindowCommandDefinition))]
    public sealed class CloseAllDockedWindowCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

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
            Command = new UICommand(CloseAllDockedWindows, CanCloseAllDockedWindows);
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