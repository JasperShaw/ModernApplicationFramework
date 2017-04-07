using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.ContextMenuDefinitions;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    public sealed class AutoHideWindowCommandDefinition : CommandDefinition
    {
        public AutoHideWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(AutoHideWindow, CanAutoHideWindow);
        }

        private bool CanAutoHideWindow()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutItem;

            if (dc?.LayoutElement == null)
                return false;

            if (dc.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null)
                return false;

            return dc.LayoutElement is LayoutAnchorable layoutItem && layoutItem.CanAutoHide && !layoutItem.IsAutoHidden;
        }

        private void AutoHideWindow()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutAnchorableItem;

            dc?.AutoHideCommand.Execute(null);

        }

        public override ICommand Command { get; }

        public override string Name => "Auto Hide";
        public override string Text => "Auto Hide";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;
    }
}
