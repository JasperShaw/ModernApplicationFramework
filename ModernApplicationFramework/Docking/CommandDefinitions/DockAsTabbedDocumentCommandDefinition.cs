﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(DockAsTabbedDocumentCommandDefinition))]
    public sealed class DockAsTabbedDocumentCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override CommandGestureCategory DefaultGestureCategory => null;

        public override string Name => Text;
        public override string Text => DockingResources.DockAsTabbedDocumentCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public DockAsTabbedDocumentCommandDefinition()
        {
            Command = new UICommand(DockAsTabbedDocument, CanDockAsTabbedDocument);
        }

        private bool CanDockAsTabbedDocument()
        {
            var dc = DockingManager.Instace.Layout.ActiveContent;
            if (dc == null)
                return false;

            var di = DockingManager.Instace.GetLayoutItemFromModel(dc);
            return di?.LayoutElement != null && di.LayoutElement.FindParent<LayoutAnchorablePane>() != null;
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