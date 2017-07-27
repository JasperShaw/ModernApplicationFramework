using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(DockWindowCommandDefinition))]
    public sealed class DockWindowCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override CommandGestureCategory DefaultGestureCategory => null;

        public override string Name => Text;
        public override string Text => DockingResources.DockWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public DockWindowCommandDefinition()
        {
            Command = new UICommand(DockWindow, CanDockWindow);
        }

        private bool CanDockWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc);

            return di?.LayoutElement?.FindParent<LayoutFloatingWindow>() != null ||
                   di?.LayoutElement?.FindParent<LayoutDocumentPane>() != null && di.LayoutElement is LayoutAnchorable || 
                   di?.LayoutElement is LayoutAnchorable layoutItem && layoutItem.IsAutoHidden;
        }

        private void DockWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc);

            if (di is LayoutAnchorableItem anchorableItem)
                anchorableItem.DockCommand.Execute(null);
            else
                di.DockAsDocumentCommand.Execute(null);
        }
    }
}