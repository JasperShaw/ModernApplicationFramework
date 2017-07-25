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
    [Export(typeof(AutoHideAllWindowsCommandDefinition))]
    public sealed class AutoHideAllWindowsCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => Text;
        public override string Text => DockingResources.AutoHideAllWindowsCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public AutoHideAllWindowsCommandDefinition()
        {
            Command = new UICommand(AutoHideAllWindows, CanAutoHideAllWindows);
        }

        private bool CanAutoHideAllWindows()
        {
            return DockingManager.Instace != null && DockingManager.Instace.Layout.Descendents()
                       .OfType<LayoutAnchorable>().Any(x => !x.IsAutoHidden);
        }

        private void AutoHideAllWindows()
        {
            var layoutAnchorables = DockingManager.Instace?.Layout.Descendents().OfType<LayoutAnchorable>();
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