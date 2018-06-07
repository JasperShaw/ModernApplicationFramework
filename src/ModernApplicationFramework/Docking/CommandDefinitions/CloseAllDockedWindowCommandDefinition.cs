using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
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
        public override ICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("CloseAllDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.CloseAllDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override Uri IconSource => new Uri(
            "/ModernApplicationFramework;component/Resources/Icons/CloseDocumentGroup.xaml",
            UriKind.RelativeOrAbsolute);

        public override string IconId => "CloseDocumentGroup";

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{343572A0-6C5A-4FFE-9E84-E1B6E68C82FB}");

        public CloseAllDockedWindowCommandDefinition()
        {
            Command = new UICommand(CloseAllDockedWindows, CanCloseAllDockedWindows);
        }

        private bool CanCloseAllDockedWindows()
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

        private void CloseAllDockedWindows()
        {
            DockingManager.Instance?._ExecuteCloseAllCommand();
        }
    }
}