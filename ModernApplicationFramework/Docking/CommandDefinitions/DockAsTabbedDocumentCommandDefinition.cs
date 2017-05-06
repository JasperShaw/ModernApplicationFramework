using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(DockAsTabbedDocumentCommandDefinition))]
    public sealed class DockAsTabbedDocumentCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override string Name => Text;
        public override string Text => DockingResources.DockAsTabbedDocumentCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public DockAsTabbedDocumentCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(DockAsTabbedDocument, CanDockAsTabbedDocument);
        }

        private bool CanDockAsTabbedDocument()
        {
            var dc = DockingManager.Instace.Layout.ActiveContent;
            if (dc == null)
                return false;

            var di = DockingManager.Instace.GetLayoutItemFromModel(dc);
            return di?.LayoutElement != null && di.LayoutElement.FindParent<LayoutDocumentPane>() == null;
        }

        private void DockAsTabbedDocument()
        {
            var dc = DockingManager.Instace.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instace.GetLayoutItemFromModel(dc);
            di?.DockAsDocumentCommand.Execute(null);
        }
    }
}