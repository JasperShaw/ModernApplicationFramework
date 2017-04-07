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
    public sealed class DockAsTabbedDocumentCommandDefinition : CommandDefinition
    {
        public DockAsTabbedDocumentCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(DockAsTabbedDocument, CanDockAsTabbedDocument);
        }

        private bool CanDockAsTabbedDocument()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutItem;

            return dc?.LayoutElement != null && dc.LayoutElement.FindParent<LayoutDocumentPane>() == null;
        }

        private void DockAsTabbedDocument()
        {
            var dc = IoC.Get<IContextMenuHost>()
                .GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu)
                .DataContext as LayoutAnchorableItem;

            dc?.DockAsDocumentCommand.Execute(null);
        }

        public override ICommand Command { get; }

        public override string Name => "Dock as Tabbed Document";
        public override string Text => "Dock as Tabbed Document";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;
    }
}
