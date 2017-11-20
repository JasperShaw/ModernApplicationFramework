using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AutoHideAllWindowsCommandDefinition))]
    public sealed class AutoHideAllWindowsCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;
        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("AutoHideAllWindowsCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.AutoHideAllWindowsCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override Guid Id => new Guid("{ABF14EC8-CC6D-4B2A-A1A1-9A0F44690266}");

        public AutoHideAllWindowsCommandDefinition()
        {
            Command = new UICommand(AutoHideAllWindows, CanAutoHideAllWindows);
        }

        private bool CanAutoHideAllWindows()
        {
            return DockingManager.Instance != null && DockingManager.Instance.Layout.Descendents()
                       .OfType<LayoutAnchorable>().Any(x => !x.IsAutoHidden);
        }

        private void AutoHideAllWindows()
        {
            var layoutAnchorables = DockingManager.Instance?.Layout.Descendents().OfType<LayoutAnchorable>();
            if (layoutAnchorables == null)
                return;

            foreach (var anchorable in layoutAnchorables.ToList())
            {
                if (!anchorable.IsAutoHidden)
                    anchorable.ToggleAutoHide();
            }

            //var di = DockingManager.Instace?.GetLayoutItemFromModel(dc) as LayoutAnchorableItem;

            //di?.AutoHideCommand.Execute(null);
        }
    }
}