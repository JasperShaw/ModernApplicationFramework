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
    [Export(typeof(DefinitionBase))]
    public sealed class DockWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => "Dock";
        public override string Text => "Dock";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public DockWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(DockWindow, CanDockWindow);
        }

        private bool CanDockWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc);

            return di?.LayoutElement?.FindParent<LayoutFloatingWindow>() != null ||
                   di?.LayoutElement?.FindParent<LayoutDocumentPane>() != null ||
                   di?.LayoutElement is LayoutAnchorable layoutItem && layoutItem.IsAutoHidden;
        }

        private void DockWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc) as LayoutAnchorableItem;

            di?.DockCommand.Execute(null);
        }
    }
}