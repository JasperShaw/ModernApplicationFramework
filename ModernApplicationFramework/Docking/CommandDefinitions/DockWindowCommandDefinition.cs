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
    public sealed class DockWindowCommandDefinition : CommandDefinition
    {
        public DockWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(DockWindow, CanDockWindow);
        }

        private bool CanDockWindow()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutItem;

            return dc?.LayoutElement?.FindParent<LayoutAnchorableFloatingWindow>() != null ||( dc?.LayoutElement is LayoutAnchorable layoutItem && layoutItem.IsAutoHidden); ;
        }

        private void DockWindow()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutAnchorableItem;

            dc?.DockCommand.Execute(null);
        }

        public override ICommand Command { get; }

        public override string Name => "Dock";
        public override string Text => "Dock";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;
    }
}
