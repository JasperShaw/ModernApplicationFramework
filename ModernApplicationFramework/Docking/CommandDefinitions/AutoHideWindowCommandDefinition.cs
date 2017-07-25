using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AutoHideWindowCommandDefinition))]
    public sealed class AutoHideWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => Text;
        public override string Text => DockingResources.AutoHideWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public AutoHideWindowCommandDefinition()
        {
            Command = new UICommand(AutoHideWindow, CanAutoHideWindow);
        }

        private bool CanAutoHideWindow()
        {
            if (DockingManager.Instace == null)
                return false;
            var dc = DockingManager.Instace.Layout.ActiveContent;
            if (dc == null)
                return false;

            var di = DockingManager.Instace.GetLayoutItemFromModel(dc);

            if (di?.LayoutElement == null)
                return false;

            if (di.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null)
                return false;

            return di.LayoutElement is LayoutAnchorable layoutItem && layoutItem.CanAutoHide &&
                   !layoutItem.IsAutoHidden;
        }

        private void AutoHideWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return;

            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc) as LayoutAnchorableItem;

            di?.AutoHideCommand.Execute(null);
        }
    }
}